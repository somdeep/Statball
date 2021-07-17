namespace Statball
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System;

    public class StatReader
    {
        public Dictionary<string, Dictionary<string, string>> stats;
        public Dictionary<string, Double> maxStats;
        public bool isp90;

        public StatReader(string[] filenames, bool isp90 = false)
        {
            stats = new Dictionary<string, Dictionary<string, string>>();
            maxStats = new Dictionary<string, double>();
            this.isp90 = isp90;

            foreach (string filename in filenames)
            {

                List<string> lines = File.ReadAllLines(filename).ToList();
                List<string> firstLevel = lines[0].Split(',').ToList();
                List<string> secondLevel = lines[1].Split(',').ToList();
                lines.RemoveAt(0);
                lines.RemoveAt(0);

                string current = string.Empty;
                for (int i = 0; i < firstLevel.Count; i++)
                {
                    if (!string.IsNullOrEmpty(firstLevel[i]))
                    {
                        current = firstLevel[i];
                        continue;
                    }

                    if (string.IsNullOrEmpty(current)) continue;

                    firstLevel[i] = current;

                }

                LoadStats(lines, firstLevel, secondLevel);
            }
        }

        public void LoadStats(List<string> lines, List<string> firstLevel, List<string> secondLevel)
        {
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] parts = line.Split(',');
                string playerKey = string.Empty;
                int partCount = 0;

                foreach (string part in parts)
                {
                    if (part.Contains("Matches") || secondLevel[partCount].Equals("Rk"))
                    {
                        partCount++;
                        continue;
                    }

                    string statKey = firstLevel[partCount] + "_" + secondLevel[partCount];

                    if (statKey.Equals("_Player") && string.IsNullOrEmpty(playerKey)) playerKey = part;

                    if (!stats.ContainsKey(playerKey))
                    {
                        stats.Add(playerKey, new Dictionary<string, string> { { statKey, part } });
                    }
                    else
                    {
                        Dictionary<string, string> playerStat = stats[playerKey];

                        if (!playerStat.ContainsKey(statKey)) playerStat.Add(statKey, part);

                        else if (Double.TryParse(playerStat[statKey], out double value) && partCount > 8 && !string.IsNullOrEmpty(part) && !statKey.Contains("%"))
                        {
                            playerStat[statKey] = (value + Double.Parse(part)).ToString();
                        }

                        stats[playerKey] = playerStat;
                    }

                    partCount++;
                }
            }
        }

        public void LoadMaxStats(double minimumFilter)
        {
            foreach (var player in stats.Values)
            {
                double _90s = Double.Parse(player["_90s"]);
                if (_90s < minimumFilter) continue;

                foreach (var stat in player)
                {
                    string statname = stat.Key;
                    double value = Per90FilteredValue(player, statname);

                    if (value != double.NaN)
                    {
                        if (maxStats.ContainsKey(statname))
                        {
                            if (maxStats[statname] < value)
                            {

                                maxStats[statname] = value;
                            }
                        }
                        else
                        {
                            maxStats[statname] = value;
                        }
                    }

                }

            }
        }

        public void TopPlayers(string statname = "Carries_Prog", int count = 20, string position = "", double minimumFilter = 20, string TeamFilter = "", string LeagueFilter = "", double ageFilter = 99, string outputFile = "results.csv")
        {
            var sortedDict = (from entry in stats
                              where !string.IsNullOrEmpty(entry.Value[statname])
                                    && ((string.IsNullOrEmpty(position)) || entry.Value["_Pos"].StartsWith(position, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry.Value["_90s"]) >= minimumFilter
                                    && ((string.IsNullOrEmpty(TeamFilter)) || entry.Value["_Squad"].Contains(TeamFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && ((string.IsNullOrEmpty(LeagueFilter)) || entry.Value["_Comp"].Contains(LeagueFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry.Value["_Age"]) <= ageFilter
                              orderby Per90FilteredValue(entry.Value, statname)
                              descending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p.Value["_Squad"], ",", p.Value["_Comp"], ",", p.Value["_Pos"], ",", p.Value["_Player"], ",", Per90FilteredValue(p.Value, statname)));

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Club,League,Position,Player,Statscore");
                foreach (string line in sortedDict)
                {
                    writer.Write(line);
                    if (count-- > 0)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        public Double Per90FilteredValue(Dictionary<string, string> entry, string statname)
        {
            if (string.IsNullOrEmpty(entry[statname])) return 0.0;

            if (Double.TryParse(entry[statname], out Double value))
            {
                if (statname.Contains("%")) return value;

                if (!isp90) return value;
                else
                {
                    return (value) / Double.Parse(entry["_90s"]);
                }

            }
            else return double.NaN;
        }

        public Double Per90FilteredValue(string statname)
        {
            if (statname.Contains("%")) return (maxStats[statname]);

            if (!isp90) return (maxStats[statname]);
            else return ((maxStats[statname]) / (maxStats["_90s"]));

        }

        public void SimilarPlayers(string[] statnames, string playerName = @"Wilfred Ndidi\Wilfred-Ndidi", int count = 20, string position = "", double minimumFilter = 20, string TeamFilter = "", string LeagueFilter = "", double ageFilter = 99, string outputFile = "similarresults.csv")
        {
            playerName = PickFirstMatchingPlayer(playerName);
            Dictionary<string, string> player = stats[playerName];

            var sortedDict = (from entry in stats
                              where ((string.IsNullOrEmpty(position)) || entry.Value["_Pos"].StartsWith(position, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry.Value["_90s"]) >= minimumFilter
                                    && !playerName.Equals(entry.Value["_Player"])
                                    && ((string.IsNullOrEmpty(TeamFilter)) || entry.Value["_Squad"].Contains(TeamFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && ((string.IsNullOrEmpty(LeagueFilter)) || entry.Value["_Comp"].Contains(LeagueFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry.Value["_Age"]) <= ageFilter
                              orderby CosineSimilarity(statnames, player, entry.Value)
                              ascending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p.Value["_Squad"], ",", p.Value["_Comp"], ",", p.Value["_Pos"], ",", p.Value["_Player"], ",", CosineSimilarity(statnames, player, p.Value)));

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Club,League,Position,Player,Statscore");
                foreach (string line in sortedDict)
                {
                    writer.Write(line);
                    if (count-- > 0)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        public Double CosineSimilarity(string[] statnames, Dictionary<string, string> player, Dictionary<string, string> entry)
        {
            double similarity = 0.0;
            double dotProduct = 0.0;
            double playerNorm = 0.0;
            double potentialNorm = 0.0;

            List<double> playerList = new List<double>();
            List<double> potentialList = new List<double>();

            foreach (string statname in statnames)
            {

                playerList.Add(Per90FilteredValue(player, statname));
                potentialList.Add(Per90FilteredValue(entry, statname));
            }

            for (int i = 0; i < playerList.Count; i++)
            {
                dotProduct += playerList[i] * potentialList[i];
                playerNorm += playerList[i] * playerList[i];
                potentialNorm += potentialList[i] * potentialList[i];
            }

            double cos = dotProduct / (Math.Sqrt(playerNorm) * Math.Sqrt(potentialNorm));
            similarity = Math.Acos(cos) * 180.0 / Math.PI;

            return similarity;
        }

        public string PickFirstMatchingPlayer(string playerName)
        {
            string name = string.Empty;

            if (string.IsNullOrEmpty(playerName)) return string.Empty;

            name = stats.Keys.First(n => n.Contains(playerName, StringComparison.InvariantCultureIgnoreCase));
            
            return name;
        }

        public void ScoutPlayer(string[] statnames, string playerName = @"", int count = 20, string position = "", double minimumFilter = 20, string TeamFilter = "", string LeagueFilter = "", double ageFilter = 99, string outputFile = "scoutresults.csv")
        {
            LoadMaxStats(minimumFilter);
            playerName = PickFirstMatchingPlayer(playerName);
            Dictionary<string, string> player = (string.IsNullOrEmpty(playerName)) ? null : stats[playerName];

            var sortedDict = (from entry in stats
                              where ((string.IsNullOrEmpty(position)) || entry.Value["_Pos"].Contains(position, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry.Value["_90s"]) >= minimumFilter
                                    && !playerName.Equals(entry.Value["_Player"])
                                    && ((string.IsNullOrEmpty(TeamFilter)) || entry.Value["_Squad"].Contains(TeamFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && ((string.IsNullOrEmpty(LeagueFilter)) || entry.Value["_Comp"].Contains(LeagueFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry.Value["_Age"]) <= ageFilter
                                    && ComputedScore(statnames, player, entry.Value) >= 0.0
                              orderby ComputedScore(statnames, player, entry.Value)
                              descending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p.Value["_Squad"], ",", p.Value["_Comp"], ",", p.Value["_Pos"], ",", p.Value["_Player"], ",", ComputedScore(statnames, player, p.Value)));

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Club,League,Position,Player,Statscore");
                foreach (string line in sortedDict)
                {
                    writer.Write(line);
                    if (count-- > 0)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        public Double ComputedScore(string[] statnames, Dictionary<string, string> player, Dictionary<string, string> entry)
        {
            double playerScore = 0.0;
            double potentialScore = 0.0;

            List<double> playerList = new List<double>();
            List<double> potentialList = new List<double>();

            foreach (string statname in statnames)
            {
                if (player != null) playerScore += (Per90FilteredValue(player, statname) / maxStats[statname]);

                potentialScore += (Per90FilteredValue(entry, statname) / maxStats[statname]);
            }

            potentialScore = potentialScore - playerScore;

            return potentialScore;
        }

    }
}