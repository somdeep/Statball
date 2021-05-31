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

            statReader.TopPlayers(statname: "Standard_Gls", count: 50, position: "", minimumFilter: 15, isp90: true);
        }
    }
}
