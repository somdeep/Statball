namespace Statball
{
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            string[] fileArray = Directory.GetFiles(@"Resources");
            StatReader statReader = new StatReader(fileArray, true);

            statReader.TopPlayers(statname: "Blocks_Int", count: 30, position: "MF", minimumFilter: 20);
            statReader.SimilarPlayers(File.ReadAllLines("Profiles/CM.txt"), playerName: @"Grillitsch", count: 30, position: "MF", LeagueFilter: "", minimumFilter: 20);
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/DM.txt"), playerName: "Grillitsch", count: 30, position: "MF", LeagueFilter: "", TeamFilter: "", ageFilter: 30, minimumFilter: 20);
        }
    }
}
