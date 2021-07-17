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

            Scraper scraper = new Scraper();

            //Player Scraping and Scouting

            string[] scrapedFiles = Directory.GetFiles(@"ScrapedPlayerResources");
            if (scrapedFiles.Length < statNames.Length)
            {

                foreach (string statname in statNames)
                {
                    scraper.PlayerScrape(statname);
                }
            }

            string[] fileArray = Directory.GetFiles(@"ScrapedPlayerResources");
            StatReader statReader = new StatReader(fileArray, isp90: true);

            statReader.TopPlayers(statname: "Blocks_Int", count: 30, position: "MF", minimumFilter: 20, outputFile: "Results/top_players.csv");
            statReader.SimilarPlayers(File.ReadAllLines("Profiles/CM.txt"), playerName: "camavinga", count: 30, position: "MF", LeagueFilter: "eng", minimumFilter: 20, outputFile: "Results/similar_players.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/CM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 30, minimumFilter: 20, outputFile: "Results/scout_players.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 30, minimumFilter: 20, outputFile: "Results/Best_u30_DMs_EPL.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 23, minimumFilter: 20, outputFile: "Results/Best_u23_DMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 99, minimumFilter: 20, outputFile: "Results/Best_AMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 99, minimumFilter: 20, outputFile: "Results/Best_AMs_EPL.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 23, minimumFilter: 20, outputFile: "Results/Best_u23_AMs.csv");

            statReader.GenerateTableauData(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 99, minimumFilter: 20, outputFile: "Results/Tableau_AMs.csv");

            //Squad scraping and scouting

            string[] scrapedSquadFiles = Directory.GetFiles(@"ScrapedSquadResources");
            if (scrapedSquadFiles.Length < statNames.Length)
            {

                foreach (string statname in statNames)
                {
                    scraper.SquadScrape(statname);
                }
            }

            fileArray = Directory.GetFiles(@"ScrapedSquadResources");
            SquadReader squadReader = new SquadReader(fileArray, isp90: true);

            squadReader.TopSquads(outputFile: "Results/TopSquads.csv");
            squadReader.SimilarSquads(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"), squadName: "Manchester Utd", count: 5);
            squadReader.ScoutSquad(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"));
            squadReader.GenerateTableauData(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"), count: 100);

        }
    }
}
