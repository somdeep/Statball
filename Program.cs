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
            Directory.CreateDirectory("ScrapedPlayerResources202526");
            string[] scrapedFiles = Directory.GetFiles(@"ScrapedPlayerResources202526");
            if (scrapedFiles.Length < statNames.Length)
            {

                foreach (string statname in statNames)
                {
                    scraper.PlayerScrape(statname);
                }
            }

            string[] fileArray = Directory.GetFiles(@"ScrapedPlayerResources202526");
            StatReader statReader = new StatReader(fileArray, isp90: true);
            Directory.CreateDirectory("2026Results");//result directory creation

            statReader.TopPlayers(statname: "Blocks_Int", count: 30, position: "MF", minimumFilter: 20, outputFile: "2026Results/top_players.csv");
            statReader.SimilarPlayers(File.ReadAllLines("Profiles/MID.txt"), playerName: "frenkie", count: 30, position: "MF", LeagueFilter: "", minimumFilter: 18, outputFile: "2026Results/similar_players.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/MID.txt"), playerName: "frenkie", count: 50, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 18, outputFile: "2026Results/scout_players.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_u30_DMs_EPL.csv");
            // statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "it", TeamFilter: "", ageFilter: 35, minimumFilter: 20, outputFile: "2026Results/Best_u23_DMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 40, position: "", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_AMs.csv");
            // statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "s", count: 30, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 99, minimumFilter: 20, outputFile: "2026Results/Best_AMs_EPL.csv");
            // statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "it", TeamFilter: "", ageFilter: 35, minimumFilter: 20, outputFile: "2026Results/Best_u23_AMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/ST.txt"), playerName: "", count: 40, position: "", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_STs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/CB.txt"), playerName: "", count: 30, position: "DF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 45, minimumFilter: 5, outputFile: "2026Results/Best_CBs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 30, position: "DF", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_FBs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/CM.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_CMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "", count: 40, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_DMs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/MID.txt"), playerName: "", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_MIDs.csv");
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/AMST.txt"), playerName: "", count: 40, position: "", LeagueFilter: "", TeamFilter: "", ageFilter: 35, minimumFilter: 5, outputFile: "2026Results/Best_AMSTs.csv");

            // statReader.GenerateTableauData(File.ReadAllLines("Profiles/AM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "it", TeamFilter: "", ageFilter: 99, minimumFilter: 20, outputFile: "2026Results/Tableau_AMs.csv");

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

            // squadReader.TopSquads(outputFile: "2026Results/TopSquads.csv");
            // squadReader.SimilarSquads(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"), squadName: "Manchester Utd", count: 5);
            // squadReader.ScoutSquad(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"));
            // squadReader.GenerateTableauData(File.ReadAllLines("Profiles/Squad_CreativeMiddle.txt"), count: 100);

        }
    }
}
