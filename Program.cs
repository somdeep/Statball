namespace Statball
{
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            string[] fileArray = Directory.GetFiles(@"Resources");
            StatReader statReader = new StatReader(fileArray, isp90: true);

            statReader.TopPlayers(statname: "Blocks_Int", count: 30, position: "MF", minimumFilter: 20);
            statReader.SimilarPlayers(File.ReadAllLines("Profiles/CM.txt"), playerName: "camavinga", count: 30, position: "MF", LeagueFilter: "eng", minimumFilter: 20);
            statReader.ScoutPlayer(File.ReadAllLines("Profiles/CM.txt"), playerName: "", count: 20, position: "MF", LeagueFilter: "eng", TeamFilter: "", ageFilter: 30, minimumFilter: 20);
        }
    }
}
