using System;

namespace Statball
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StatReader statReader = new StatReader("Resources/PlayerPossession.csv");
            statReader.LoadStats();
            statReader.TopPlayers("Drib_Succ", 50);
        }
    }
}
