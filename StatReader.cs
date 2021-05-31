namespace Statball
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System;

    public class StatReader
    {
        public List<string> firstLevel;
        public List<string> secondLevel;
        public List<string> lines;
        public List<Dictionary<string, string>> stats;

        public StatReader(string filename)
        {
            lines = File.ReadAllLines(filename).ToList();
            firstLevel = lines[0].Split(',').ToList();
            secondLevel = lines[1].Split(',').ToList();
            lines.RemoveAt(0);
            lines.RemoveAt(0);

            stats = new List<Dictionary<string, string>>();

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
        }

        public void LoadStats()
        {
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                int partCount = 0;
                Dictionary<string, string> playerStat = new Dictionary<string, string>();

                foreach (string part in parts)
                {
                    string statKey = firstLevel[partCount] + "_" + secondLevel[partCount];
                    playerStat.Add(statKey, part);
                    partCount++;
                }

                stats.Add(playerStat);
            }
        }

        public void TopPlayers(string statname = "Carr_Prog", int count = 20, string position = "", double minimumFilter = 20, string TeamFilter="", string LeagueFilter="", bool isp90 = false, string outputFile = "results.csv")
        {
            var sortedDict = (from entry in stats
                              where !string.IsNullOrEmpty(entry[statname])
                                    && ((string.IsNullOrEmpty(position)) || entry["_Pos"].Contains(position, StringComparison.InvariantCultureIgnoreCase))
                                    && Double.Parse(entry["_90s"]) >= minimumFilter
                                    && 
                              orderby Per90FilteredValue(entry, statname, isp90)
                              descending
                              select entry)
                     .Take(count)
                     .Select(p => string.Concat(p["_Squad"], ",", p["_Player"], ",", Per90FilteredValue(p, statname, isp90)));

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