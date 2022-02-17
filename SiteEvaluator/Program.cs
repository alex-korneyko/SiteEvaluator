using SiteEvaluator.Crawler;
using SiteEvaluator.PageLoader;
using SiteEvaluator.SiteMapExplorer;

var commandLineArgs = Environment.GetCommandLineArgs();

var baseUrl = "https://www.ukad-group.com/";

ISiteCrawler siteCrawler = new SiteCrawler(new HttpContentLoader(), settings =>
{
    settings.IncludeNofollowLinks = false;
    settings.LogToConsole = false;
    settings.PrintResult = true;
});

var crawlerResults = await siteCrawler.CrawlAsync(baseUrl);

Console.WriteLine($"\nTotal crawled pages: {crawlerResults.Count}");

ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(new HttpContentLoader(), settings =>
{
    settings.PrintResult = true;
});

var siteMap = await siteMapExplorer.ExploreAsync(baseUrl);

Console.WriteLine($"\nTotal in sitemap.xml: {siteMap.UrlSet?.Length}");

