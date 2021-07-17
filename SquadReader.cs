namespace Statball
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System;

    public class SquadReader
    {
        public Dictionary<string, Dictionary<string, string>> stats;
        public Dictionary<string, Double> maxStats;
        public bool isp90;

        public SquadReader(string[] filenames, bool isp90 = false)
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
                string squadKey = string.Empty;
                int partCount = 0;

                foreach (string part in parts)
                {
                    if (part.Contains("Matches") || secondLevel[partCount].Equals("Rk"))
                    {
                        partCount++;
                        continue;
                    }

                    string statKey = firstLevel[partCount] + "_" + secondLevel[partCount];

                    if (statKey.Equals("_Squad") && string.IsNullOrEmpty(squadKey)) squadKey = part;

                    if (!stats.ContainsKey(squadKey))
                    {
                        stats.Add(squadKey, new Dictionary<string, string> { { statKey, part } });
                    }
                    else
                    {
                        Dictionary<string, string> squadStat = stats[squadKey];

                        if (!squadStat.ContainsKey(statKey)) squadStat.Add(statKey, part);

                        else if (Double.TryParse(squadStat[statKey], out double value) && partCount > 8 && !string.IsNullOrEmpty(part) && !statKey.Contains("%"))
                        {
                            squadStat[statKey] = (value + Double.Parse(part)).ToString();
                        }

                        stats[squadKey] = squadStat;
                    }

                    partCount++;
                }
            }
        }

        public void LoadMaxStats(double minimumFilter)
        {
            maxStats = new Dictionary<string, double>();

            foreach (var squad in stats.Values)
            {
                double _90s = Double.Parse(squad["_90s"]);
                if (_90s < minimumFilter) continue;

                foreach (var stat in squad)
                {
                    string statname = stat.Key;
                    double value = Per90FilteredValue(squad, statname);

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

        public void TopSquads(string statname = "Carries_Prog", int count = 20, double minimumFilter = 0, string LeagueFilter = "", string outputFile = "Results/TopSquads.csv")
        {
            var sortedDict = (from entry in stats
                              where !string.IsNullOrEmpty(entry.Value[statname])
                                    && Double.Parse(entry.Value["_90s"]) >= minimumFilter
                                    && ((string.IsNullOrEmpty(LeagueFilter)) || entry.Value["_Comp"].Contains(LeagueFilter, StringComparison.InvariantCultureIgnoreCase))
                              orderby Per90FilteredValue(entry.Value, statname)
                              descending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p.Value["_Comp"], ",", p.Value["_Squad"], ",", Per90FilteredValue(p.Value, statname)));

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("League,Club,Statscore");
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

        public void SimilarSquads(string[] statnames, string squadName = @"Manchester Utd", int count = 20, double minimumFilter = 0, string LeagueFilter = "", string outputFile = "Results/SimilarSquads.csv")
        {
            squadName = PickFirstMatchingSquad(squadName);
            Dictionary<string, string> squad = stats[squadName];

            var sortedDict = (from entry in stats
                              where Double.Parse(entry.Value["_90s"]) >= minimumFilter
                                    && !squadName.Equals(entry.Value["_Squad"])
                                    && ((string.IsNullOrEmpty(LeagueFilter)) || entry.Value["_Comp"].Contains(LeagueFilter, StringComparison.InvariantCultureIgnoreCase))
                              orderby CosineSimilarity(statnames, squad, entry.Value)
                              ascending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p.Value["_Comp"], ",", p.Value["_Squad"], ",", CosineSimilarity(statnames, squad, p.Value)));

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("League,Club,Statscore");
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

        public Double CosineSimilarity(string[] statnames, Dictionary<string, string> squad, Dictionary<string, string> entry)
        {
            double similarity = 0.0;
            double dotProduct = 0.0;
            double squadNorm = 0.0;
            double potentialNorm = 0.0;

            List<double> squadList = new List<double>();
            List<double> potentialList = new List<double>();

            foreach (string statname in statnames)
            {

                squadList.Add(Per90FilteredValue(squad, statname));
                potentialList.Add(Per90FilteredValue(entry, statname));
            }

            for (int i = 0; i < squadList.Count; i++)
            {
                dotProduct += squadList[i] * potentialList[i];
                squadNorm += squadList[i] * squadList[i];
                potentialNorm += potentialList[i] * potentialList[i];
            }

            double cos = dotProduct / (Math.Sqrt(squadNorm) * Math.Sqrt(potentialNorm));
            similarity = Math.Acos(cos) * 180.0 / Math.PI;

            return similarity;
        }

        public string PickFirstMatchingSquad(string squadName)
        {
            string name = string.Empty;

            if (string.IsNullOrEmpty(squadName)) return string.Empty;

            name = stats.Keys.First(n => n.Contains(squadName, StringComparison.InvariantCultureIgnoreCase));

            return name;
        }

        public void ScoutSquad(string[] statnames, string squadName = @"", int count = 20, double minimumFilter = 0, string LeagueFilter = "", string outputFile = "Results/ScoutedTeams.csv")
        {
            LoadMaxStats(minimumFilter);
            squadName = PickFirstMatchingSquad(squadName);
            Dictionary<string, string> squad = (string.IsNullOrEmpty(squadName)) ? null : stats[squadName];

            var sortedDict = (from entry in stats
                              where Double.Parse(entry.Value["_90s"]) >= minimumFilter
                                    && !squadName.Equals(entry.Value["_Squad"])
                                    && ((string.IsNullOrEmpty(LeagueFilter)) || entry.Value["_Comp"].Contains(LeagueFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && ComputedScore(statnames, squad, entry.Value) >= 0.0
                              orderby ComputedScore(statnames, squad, entry.Value)
                              descending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p.Value["_Comp"], ",", p.Value["_Squad"], ",", ComputedScore(statnames, squad, p.Value)));

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("League,Club,Statscore");
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

        public Double ComputedScore(string[] statnames, Dictionary<string, string> squad, Dictionary<string, string> entry)
        {
            double squadScore = 0.0;
            double potentialScore = 0.0;

            List<double> squadList = new List<double>();
            List<double> potentialList = new List<double>();

            foreach (string statname in statnames)
            {
                if (squad != null) squadScore += (Per90FilteredValue(squad, statname) / maxStats[statname]);

                potentialScore += (Per90FilteredValue(entry, statname) / maxStats[statname]);
            }

            potentialScore = potentialScore - squadScore;

            return potentialScore;
        }

        public void GenerateTableauData(string[] statnames, string squadName = @"", int count = 20, double minimumFilter = 20, string LeagueFilter = "", string outputFile = "Results/TableauSquads.csv")
        {
            LoadMaxStats(minimumFilter);
            squadName = PickFirstMatchingSquad(squadName);
            Dictionary<string, string> squad = (string.IsNullOrEmpty(squadName)) ? null : stats[squadName];

            var sortedDict = (from entry in stats
                              where Double.Parse(entry.Value["_90s"]) >= minimumFilter
                                    && !squadName.Equals(entry.Value["_Squad"])
                                    && ((string.IsNullOrEmpty(LeagueFilter)) || entry.Value["_Comp"].Contains(LeagueFilter, StringComparison.InvariantCultureIgnoreCase))
                                    && ComputedScore(statnames, squad, entry.Value) >= 0.0
                              orderby ComputedScore(statnames, squad, entry.Value)
                              descending
                              select entry)
                     .Take(count);

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.Write("Squad");

                foreach (string statname in statnames)
                {
                    writer.Write("," + statname);
                }

                writer.WriteLine();

                foreach (var line in sortedDict)
                {
                    var linesquad = line.Value;

                    writer.Write(linesquad["_Squad"]);

                    foreach (string statname in statnames)
                    {
                        writer.Write("," + linesquad[statname]);
                    }

                    if (count-- > 0)
                    {
                        writer.WriteLine();
                    }
                }
            }

        }

    }
}