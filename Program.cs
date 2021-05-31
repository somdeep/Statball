using System;

namespace Statball
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StatReader statReader = new StatReader(@"Resources\PlayerGoalAndShotCreation.csv");
            statReader.LoadStats();
            statReader.TopPlayers(statname: "SCA_SCA", count: 50, position: "mf", minimumFilter: 15, isp90: true);
        }
    }
}
