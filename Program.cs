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

            statReader.TopPlayers(statname: "Blocks_Int", count: 50, position: "MF", minimumFilter: 20, isp90: true);
            statReader.SimilarPlayers(File.ReadAllLines("Profiles/DM.txt"), playerName: @"Wilfred Ndidi\Wilfred-Ndidi", count: 50, position: "MF", LeagueFilter: "eng Premier League", minimumFilter: 15, isp90: true);
        }
    }
}
