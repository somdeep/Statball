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

            headerNames.Remove("Matches");
            columnSpans.RemoveAt(columnSpans.Count - 1);
            overHeaderNames.RemoveAt(overHeaderNames.Count - 1);


        }
    }

    public class Row
    {
        public string Title { get; set; }
    }
}