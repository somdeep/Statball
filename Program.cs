namespace Statball
{
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            string[] statNames = { "passing", "shooting", "passing_types", "gca", "defense", "possession", "misc", "stats" };
            // "playingtime", "keepers","keepersadv"};

            string[] scrapedFiles = Directory.GetFiles(@"ScrapedResources");
            if (scrapedFiles.Length < statNames.Length)
            {
                Scraper scraper = new Scraper();
                foreach (string statname in statNames)
                {
                    scraper.Scrape(statname);
                }
            }

            string[] fileArray = Directory.GetFiles(@"ScrapedResources");
            StatReader statReader = new StatReader(fileArray, isp90: true);

            statReader.TopPlayers(statname: "Blocks_Int", count: 30, position: "MF", minimumFilter: 20, outputFile: "Results/top_players.csv");
            statReader.SimilarPlayers(File.ReadAllLines("Profiles/CM.txt"), playerName: "camavinga", count: 30, position: "MF", LeagueFilter: "eng", minimumFilter: 20, outputFile: "Results/similar_players.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/CM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 30, minimumFilter: 20, outputFile: "Results/scout_players.csv");
        }
    }
}
