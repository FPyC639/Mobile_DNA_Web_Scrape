using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;

public abstract class Reader_HTML
{

	abstract public List<string> read_title();
	
	
}

public class Process_HTML : Reader_HTML
{

	public string url { get; set; }
	public string year { get; set; }
	private string query_string;
	private HtmlDocument doc = new HtmlDocument();
	private HtmlDocument subdoc = new HtmlDocument();


    public Process_HTML() 
	{
		url = "https://mobilednajournal.biomedcentral.com/articles";

    }
    /// <summary>
	/// The purpose of this method is that it creates Uri's that can be used by the HTML Web Scraping libraries.
	/// </summary>
    private Uri creator(string url)
	{
		
		return new Uri(url);
	}


    /// <summary>
    /// The purpose of this method is that it parses the webpage and gathers the necessary components for individuals to collect data.
    /// </summary>
    private async Task<HtmlDocument> parse_webpage()
	{
        var webGet = new HttpClient();
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
			doc = null;
        }
		return doc;
    }
	/// <summary>
	/// The purpose of this method is to gather the titles for the Mobile DNA journal.
	/// </summary>
	public override List<string> read_title()
	{
		var doc = parse_webpage().Result;
		var list = new List<string>();
		var titles_nodes = doc.DocumentNode.SelectNodes(@"//h3[@class='c-listing__title']");
		foreach( var title in titles_nodes )
		{
			list.Add(title.InnerText);
		}        
		return list;
    }

	public async void two_deep_extractor()
	{
        var doc = parse_webpage().Result;
        var a_href = doc.DocumentNode.SelectNodes(@"//a[@itemprop='url']");
		foreach( var a in a_href)
		{
			Console.WriteLine(a.Attributes["href"].Value);
		}
	}
}
