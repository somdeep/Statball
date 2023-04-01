namespace Statball
{
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            string[] statNames = { "passing", "shooting", "passing_types", "gca", "defense", "possession", "misc", "standard" };
            // "playingtime", "keepers","keepersadv"};

            Scraper scraper = new Scraper();

            //Player Scraping and Scouting
            Directory.CreateDirectory("ScrapedPlayerResources1");
            string[] scrapedFiles = Directory.GetFiles(@"ScrapedPlayerResources1");
            if (scrapedFiles.Length < statNames.Length)
            {

                foreach (string statname in statNames)
                {
                    scraper.PlayerScrape(statname);
                }
            }

            string[] fileArray = Directory.GetFiles(@"ScrapedPlayerResources1");
            StatReader statReader = new StatReader(fileArray, isp90: true);
            Directory.CreateDirectory("2023Results");//result directory creation

            statReader.TopPlayers(statname: "Blocks_Int", count: 30, position: "MF", minimumFilter: 20, outputFile: "2023Results/top_players.csv");
            // statReader.SimilarPlayers(File.ReadAllLines("Profiles/DM.txt"), playerName: "pogba", count: 30, position: "MF", LeagueFilter: "", minimumFilter: 18, outputFile: "2023Results/similar_players.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/ST.txt"), playerName: "", count: 50, position: "FW", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 15, outputFile: "2023Results/scout_players.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 30, minimumFilter: 18, outputFile: "2023Results/Best_u30_DMs_EPL.csv");
            // statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 20, outputFile: "2023Results/Best_u23_DMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 18, outputFile: "2023Results/Best_AMs.csv");
            // statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 99, minimumFilter: 20, outputFile: "2023Results/Best_AMs_EPL.csv");
            // statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 20, outputFile: "2023Results/Best_u23_AMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/ST.txt"), playerName: "", count: 30, position: "FW", LeagueFilter: "eng", TeamFilter: "", ageFilter: 40, minimumFilter: 18, outputFile: "2023Results/Best_STs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/CB.txt"), playerName: "", count: 30, position: "DF", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 18, outputFile: "2023Results/Best_CBs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 30, position: "DF", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 18, outputFile: "2023Results/Best_FBs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/CM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 18, outputFile: "2023Results/Best_CMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 18, outputFile: "2023Results/Best_DMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/MID.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 18, outputFile: "2023Results/Best_MIDs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AMST.txt"), playerName: "", count: 30, position: "", LeagueFilter: "", TeamFilter: "", ageFilter: 40, minimumFilter: 18, outputFile: "2023Results/Best_AMSTs.csv");

            // statReader.GenerateTableauData(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 99, minimumFilter: 20, outputFile: "2023Results/Tableau_AMs.csv");

            //Squad scraping and scouting

            // string[] scrapedSquadFiles = Directory.GetFiles(@"ScrapedSquadResources");
            // if (scrapedSquadFiles.Length < statNames.Length)
            // {

            //     foreach (string statname in statNames)
            //     {
            //         scraper.SquadScrape(statname);
            //     }
            // }

            // fileArray = Directory.GetFiles(@"ScrapedSquadResources");
            // SquadReader squadReader = new SquadReader(fileArray, isp90: true);

            // squadReader.TopSquads(outputFile: "2023Results/TopSquads.csv");
            // squadReader.SimilarSquads(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"), squadName: "Manchester Utd", count: 5);
            // squadReader.ScoutSquad(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"));
            // squadReader.GenerateTableauData(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"), count: 100);

        }
    }
}
