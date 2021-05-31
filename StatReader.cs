namespace Statball
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System;

    public class StatReader
    {
        public Dictionary<string, Dictionary<string, string>> stats;

        public StatReader(string[] filenames)
        {
            stats = new Dictionary<string, Dictionary<string, string>>();

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

        public void TopPlayers(string statname = "Carr_Prog", int count = 20, string position = "", double minimumFilter = 20, string TeamFilter = "", string LeagueFilter = "", bool isp90 = false, string outputFile = "results.csv")
        {
            var sortedDict = (from entry in stats
                              where !string.IsNullOrEmpty(entry.Value[statname])
                                    && ((string.IsNullOrEmpty(position)) || entry.Value["_Pos"].Contains(position, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry.Value["_90s"]) >= minimumFilter
                              orderby Per90FilteredValue(entry.Value, statname, isp90)
                              descending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p.Value["_Squad"], ",", p.Value["_Comp"], ",", p.Value["_Pos"], ",", p.Value["_Player"], ",", Per90FilteredValue(p.Value, statname, isp90)));

            count = sortedDict.Count() - 1;
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
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

        public Double Per90FilteredValue(Dictionary<string, string> entry, string statname, bool isp90)
        {
            if (!isp90) return Double.Parse(entry[statname]);
            else return (Double.Parse(entry[statname]) / Double.Parse(entry["_90s"]));

        }
    }
}