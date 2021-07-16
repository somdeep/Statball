namespace Statball
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System;
    using CsvHelper;
    using HtmlAgilityPack;
    using System.Globalization;

    public class Scraper
    {
        public void Scrape()
        {
            string statname = "shooting";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"https://fbref.com/en/comps/Big5/{statname}/players/Big-5-European-Leagues-Stats");

            List<string> overHeaderNames = doc.DocumentNode.SelectNodes($"//table[@id='stats_{statname}']//thead//tr")[0].ChildNodes.Where(n => n.Name == "th").Select(child => child.InnerText).ToList();

            List<int> columnSpans = new List<int>();
            var columns = doc.DocumentNode.SelectNodes($"//table[@id='stats_{statname}']//thead//tr[@class='over_header']//th").Select(n => n.Attributes).ToList();
            foreach (var column in columns)
            {
                int value = column.Any(c => c.Name == "colspan") ? Convert.ToInt32(column["colspan"].Value) : 1;
                columnSpans.Add(value);
            }

            List<string> headerNames = doc.DocumentNode.SelectNodes($"//table[@id='stats_{statname}']//thead//tr")[1].ChildNodes.Where(n => n.Name == "th").Select(child => child.InnerText).ToList();

            int count = 0;
            using (StreamWriter writer = new StreamWriter("output.txt"))
            {
                foreach (string name in overHeaderNames)
                {
                    int span = columnSpans[count++];
                    writer.Write(name + ",");
                    for (int i = 1; i < span; i++)
                    {
                        if (count == overHeaderNames.Count - 1 && i == span - 1) continue;

                        writer.Write(",");
                    }
                }

                writer.WriteLine();

                count = headerNames.Count - 1;
                foreach (string name in headerNames)
                {
                    if (count > 0) writer.Write(name + ",");
                    else writer.Write(name);

                    count--;
                }

                writer.WriteLine();
            }

        }
    }

    public class Row
    {
        public string Title { get; set; }
    }
}