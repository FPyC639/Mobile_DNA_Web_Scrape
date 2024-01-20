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
/// <summary>
/// Abstract class constructing Template to Process HTML for certain WebPage.
/// </summary>
public abstract class Reader_HTML
{
	abstract protected Uri creator(string url);
	abstract protected Task<HtmlDocument> parse_webpage();
	abstract public List<string> read_title();
    abstract public List<string> read_authors(HtmlDocument doc);
    abstract public List<string> read_access(HtmlDocument doc);
    abstract public Task<List<List<string>>> two_deep_extractor();
    abstract public Task<List<List<string>>> two_deep_extractor_V2();
    abstract public void write_html_csv();
    abstract public void write_access_csv();



}
/// <summary>
/// Concrete class inheriting ReaderHTML abstract class that implements abstract methods for Mobile DNA website.
/// </summary>
public class Process_HTML : Reader_HTML
{

	public string url { get; set; }
    public string query { get; set; }
    public string year;
    public void set_year(String year)
    {
        switch (year)
        {
            case "2010":
                this.year = "1"; break;
            case "2011":
                this.year = "2"; break;
            case "2012":
                this.year = "3"; break;
            case "2013":
                this.year = "4"; break;
            case "2014":
                this.year = "5"; break;
            case "2015":
                this.year = "6"; break;
            case "2016":
                this.year = "7"; break;
            case "2017":
                this.year = "8"; break;
            case "2018":
                this.year = "9"; break;
            case "2019":
                this.year = "10"; break;
            case "2020":
                this.year = "11"; break;
            case "2021":
                this.year = "12"; break;
            case "2022":
                this.year = "13"; break;
            case "2023":
                this.year = "14"; break;
            default:
                this.year = null;
                throw new Exception("Year must be between 2010 and 2023.");
                break;
        }
    }
    
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
    protected override Uri creator(string url)
	{
		
		return new Uri(url);
	}


    /// <summary>
    /// The purpose of this method is that it parses the webpage and gathers the necessary components for individuals to collect data.
    /// </summary>
    protected override async Task<HtmlDocument> parse_webpage()
	{
        var webGet = new HttpClient();
        this.query_string = this.url + string.Format(@"?query={0}&volume={1}&searchType=&tab=keyword", this.query ,this.year);
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
    public override List<string> read_authors(HtmlDocument doc)
    {
        var list = new List<string> ();
        var author_name = doc.DocumentNode.SelectNodes(@"//a[@data-test='author-name']");
        foreach( var author in author_name)
        {
            list.Add(author.InnerText);
        }
        return list;
    }

    public override List<string> read_access(HtmlDocument doc)
    {
        var list = new List<string>();
        var access = doc.DocumentNode.SelectNodes(
            @"//ul[@class='c-article-metrics-bar u-list-reset']" +
            "//li[@class=' c-article-metrics-bar__item']" +
            "//p[@class='c-article-metrics-bar__count']");
        list.Add(access[0].InnerText);
        
        return list;
    }

	public override async Task<List<List<string>>> two_deep_extractor()
	{
		var webpage_hrefs = new List<string>();
        var webpage_authors = new List<List<string>>();
        var webGet = new HttpClient();
        var doc = parse_webpage().Result;
        var a_href = doc.DocumentNode.SelectNodes(@"//h3//a[@itemprop='url']");
		foreach( var a in a_href)
		{
			webpage_hrefs.Add(string.Concat(this.url.Replace("/articles",""),a.Attributes["href"].Value));
		}
		foreach( var str in webpage_hrefs)
		{
            var response = await webGet.GetAsync(creator(str));
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
            var list = read_authors(doc);
            webpage_authors.Add(list);
        }
        return webpage_authors;
    }

    public override async Task<List<List<string>>> two_deep_extractor_V2()
    {
        var accesscounts = new List<List<string>>();
        var webpage_hrefs = new List<string>();
        var webGet = new HttpClient();
        var doc = parse_webpage().Result;
        var a_href = doc.DocumentNode.SelectNodes(@"//h3//a[@itemprop='url']");
        foreach (var a in a_href)
        {
            webpage_hrefs.Add(string.Concat(this.url.Replace("/articles", ""), a.Attributes["href"].Value));
        }
        foreach (var str in webpage_hrefs)
        {
            var response = await webGet.GetAsync(creator(str));
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
            var list = read_access(doc);
            accesscounts.Add(list);
        }
        return accesscounts;
    }

    public override void write_html_csv()
    {
        var csv = new StringBuilder();
        string newLine = null;
        var titles = read_title();
        var authors = two_deep_extractor().Result;
        csv.AppendLine("Title;Authors");
        List<string> mergedTitlesAndAuthors = new List<string>();
        for (int i = 0; i < titles.Count; i++)
        {
            string title = titles[i];
            List<string> authorList = authors[i];
            string authorsString = string.Join(", ", authorList);
            string mergedString = $"{title.Trim()}; {authorsString.Trim()}";
            mergedTitlesAndAuthors.Add(mergedString);
        }
        foreach(string merger in mergedTitlesAndAuthors)
        {
            csv.AppendLine(merger);
        }
        File.WriteAllText(@"C:\Users\joses\Source\Repos\Mobile_DNA_Web_Scrape\MobileDNA\Data4.txt", csv.ToString());
    }
    public override void write_access_csv()
    {
        var csv = new StringBuilder();
        string newLine = null;
        var titles = read_title();
        var access = two_deep_extractor_V2().Result;
        csv.AppendLine("Titles;Accesses");
        List<string> mergedTitlesAndAccess = new List<string>();
        for (int i = 0; i < titles.Count; i++)
        {
            string title = titles[i];
            List<string> access_i = access[i];
            string access_ii = access_i[0];
            mergedTitlesAndAccess.Add(String.Format("{0};{1}", title.Trim(),access_ii));
        }
        foreach (string merger in mergedTitlesAndAccess)
        {
            csv.AppendLine(merger);
        }
        File.WriteAllText(@"C:\Users\joses\Source\Repos\Mobile_DNA_Web_Scrape\MobileDNA\AccessData1.txt", csv.ToString());
    }

}
