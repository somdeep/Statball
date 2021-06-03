namespace Statball
{
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            string[] fileArray = Directory.GetFiles(@"Resources");
            StatReader statReader = new StatReader(fileArray);

            statReader.TopPlayers(statname: "Blocks_Int", count: 20, position: "MF", minimumFilter: 20, isp90: true);
            statReader.SimilarPlayers(File.ReadAllLines("Profiles/DM.txt"), playerName: @"bissouma", count: 20, position: "MF", LeagueFilter: "", minimumFilter: 20, isp90: true);
        }
    }
}
