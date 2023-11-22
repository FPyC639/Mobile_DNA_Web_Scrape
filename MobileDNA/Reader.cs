﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;

public abstract class Reader_HTML
{

	
	public struct plotter_feeder {
		public List<string> lines;
		public List<string> lines1;
		public List<string> lines2;
		public List<string> lines3;
	}
	
}

public class Process_HTML : Reader_HTML
{

	public string url { get; set; }
	public string year { get; set; }
	private string query_string;
	public Process_HTML() 
	{
		url = "https://mobilednajournal.biomedcentral.com/articles";

    }

	private Uri creator(string url)
	{
		return new Uri(url);
	}
	public async void read_html()
	{
		var webGet = new HttpClient();
        HtmlDocument doc = new HtmlDocument();
		this.query_string = this.url + string.Format(@"?query={0}&volume=&searchType=&tab=keyword", year);
		var response = await webGet.GetAsync(creator(this.query_string));
        if (response.IsSuccessStatusCode)
        {
            // Read the response content asynchronously 
            response.Content.Headers.ContentType.CharSet = @"UTF-8";
            string content = await response.Content.ReadAsStringAsync();

            
            doc.LoadHtml(content);

        }
        else
        {
            Console.WriteLine($"Failed with status code: {response.StatusCode}");
        }
		var titles_nodes = doc.DocumentNode.SelectNodes(@"//h3[@class='c-listing__title']");
		Console.WriteLine(titles_nodes.Count);
        var csv = new StringBuilder();
        string newLine = null;
        csv.AppendLine("Paper Index; Title");
		int count = 1;
        foreach (var node in  titles_nodes)
		{
			newLine = string.Format("{0};{1}", count, node.InnerText.Trim());
			csv.AppendLine(newLine);
			count++;
        }
		File.WriteAllText(@"C:\Users\joses\Source\Repos\Mobile_DNA_Web_Scrape\MobileDNA\Data.txt", csv.ToString());
    }
}
