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

        public void TopPlayers(string statname = "Carr_Prog", int count = 20, string outputFile = "results.txt")
        {
            IEnumerable<string> sortedDict = (from entry in stats 
                                where !string.IsNullOrEmpty(entry[statname]) 
                                orderby Int32.Parse(entry[statname]) 
                                descending select entry)
                     .Take(count)
                     .Select(p => p["_Player"]);

            File.WriteAllLines(outputFile, sortedDict);

        }
    }
}