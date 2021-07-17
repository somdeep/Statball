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
        public void Scrape(string statname = "passing")
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"https://fbref.com/en/comps/Big5/{statname}/players/Big-5-European-Leagues-Stats");


            if (statname == "stats") statname = "standard";

            List<string> overHeaderNames = doc.DocumentNode.SelectNodes($"//table[@id='stats_{statname}']//thead//tr")[0].ChildNodes.Where(n => n.Name == "th").Select(child => child.InnerText).ToList();

            List<int> columnSpans = new List<int>();
            var columns = doc.DocumentNode.SelectNodes($"//table[@id='stats_{statname}']//thead//tr[@class='over_header']//th").Select(n => n.Attributes).ToList();
            foreach (var column in columns)
            {
                int value = column.Any(c => c.Name == "colspan") ? Convert.ToInt32(column["colspan"].Value) : 1;
                columnSpans.Add(value);
            }

            List<string> headerNames = doc.DocumentNode.SelectNodes($"//table[@id='stats_{statname}']//thead//tr")[1].ChildNodes.Where(n => n.Name == "th").Select(child => child.InnerText).ToList();

            List<HtmlNodeCollection> playerNodes = doc.DocumentNode.SelectNodes($"//table[@id='stats_{statname}']//tbody//tr").Select(node => node.ChildNodes).ToList();

            int count = 0;
            using (StreamWriter writer = new StreamWriter($"ScrapedPlayerResources/Player_{statname}.csv"))
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
                count = playerNodes.Count - 1;
                foreach (var playerNode in playerNodes)
                {
                    string firstNode = playerNode.Nodes().First().InnerText;
                    if (firstNode == "Rk") continue;

                    foreach (var node in playerNode)
                    {

                        if (node.InnerText == "Matches") writer.Write(node.InnerText);

                        else
                        {
                            string value = node.Attributes.Where(n => n.Name == "data-stat").First().Value;

                            if (value == "position") writer.Write(node.InnerText.Replace(",", "") + ",");
                            else writer.Write(node.InnerText + ",");
                        }
                    }

                    count--;
                    if (count > 2) writer.WriteLine();
                }
            }

        }

        public void SquadScrape(string statname = "passing")
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"https://fbref.com/en/comps/Big5/{statname}/squads/Big-5-European-Leagues-Stats");


            if (statname == "stats") statname = "standard";

            List<string> overHeaderNames = doc.DocumentNode.SelectNodes($"//table[@id='stats_squads_{statname}_for']//thead//tr")[0].ChildNodes.Where(n => n.Name == "th").Select(child => child.InnerText).ToList();

            List<int> columnSpans = new List<int>();
            var columns = doc.DocumentNode.SelectNodes($"//table[@id='stats_squads_{statname}_for']//thead//tr[@class='over_header']//th").Select(n => n.Attributes).ToList();
            foreach (var column in columns)
            {
                int value = column.Any(c => c.Name == "colspan") ? Convert.ToInt32(column["colspan"].Value) : 1;
                columnSpans.Add(value);
            }

            List<string> headerNames = doc.DocumentNode.SelectNodes($"//table[@id='stats_squads_{statname}_for']//thead//tr")[1].ChildNodes.Where(n => n.Name == "th").Select(child => child.InnerText).ToList();

            List<HtmlNodeCollection> squadNodes = doc.DocumentNode.SelectNodes($"//table[@id='stats_squads_{statname}_for']//tbody//tr").Select(node => node.ChildNodes).ToList();

            int count = 0;
            using (StreamWriter writer = new StreamWriter($"ScrapedSquadResources/Squad_{statname}.csv"))
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
                count = squadNodes.Count - 1;
                foreach (var squadNode in squadNodes)
                {
                    string firstNode = squadNode.Nodes().First().InnerText;
                    if (firstNode == "Rk") continue;

                    int nodeCount = squadNode.Count - 1;
                    foreach (var node in squadNode)
                    {

                        if (nodeCount == 0) writer.Write(node.InnerText);

                        else
                        {
                            string value = node.Attributes.Where(n => n.Name == "data-stat").First().Value;

                            if (value == "position") writer.Write(node.InnerText.Replace(",", "") + ",");
                            else writer.Write(node.InnerText + ",");
                        }

                        nodeCount--;
                    }

                    count--;
                    if (count > 2) writer.WriteLine();
                }
            }

        }
    }

}