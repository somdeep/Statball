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

            statReader.TopPlayers(statname: "Tackles_Mid 3rd", count: 50, position: "MF", minimumFilter: 20, isp90: true);
        }
    }
}
