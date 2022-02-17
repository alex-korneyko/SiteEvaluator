using System.Text;
using SiteEvaluator.Crawler;
using SiteEvaluator.PageLoader;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExplorer;

Console.Write("Please, enter host URL for evaluate: ");
var hostUrl = Console.ReadLine();
if (hostUrl == null) return;

var httpContentLoader = new HttpContentLoader();
ISiteCrawler siteCrawler = new SiteCrawler(httpContentLoader, settings =>
{
    settings.IncludeNofollowLinks = false;
    settings.LogToConsole = false;
    settings.PrintResult = false;
});

var crawlerResults = await siteCrawler.CrawlAsync(hostUrl);

var finalReport = new StringBuilder($"Urls(html documents) found after crawling a website: {crawlerResults.Count}");

Console.WriteLine($"Total crawled pages: {crawlerResults.Count}\n");

ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(httpContentLoader, settings =>
{
    settings.PrintResult = false;
});

var siteMapExploreResult = await siteMapExplorer.ExploreAsync(hostUrl);

var pageLoadResultsForOnlyFromSiteMapLinks = new List<ContentLoadResult>();

if (siteMapExploreResult.IsSuccess)
{
    Console.WriteLine($"Total in sitemap.xml: {siteMapExploreResult.SiteMap.UrlSet?.Length}\n");

    var siteMapUrls = siteMapExploreResult.SiteMap.UrlSet!
        .Where(url => url.Loc != null)
        .Select(url => new ContentLoadResult(url.Loc!))
        .ToList();
    
    ResultsComparer.DifferenceReport(crawlerResults, siteMapUrls);
    
    var onlyInSiteMap = ResultsComparer.SubtractLists(siteMapUrls, crawlerResults)
        .Select(item => item.PageUrl);
    
    var contentLoader = new HttpContentLoader();
    foreach (var url in onlyInSiteMap)
    {
        pageLoadResultsForOnlyFromSiteMapLinks.Add(await contentLoader.LoadContentAsync(url));
    }

    finalReport.Append($"\nUrls found in sitemap: {siteMapExploreResult.SiteMap.UrlSet?.Length}");
}

ConsoleMessage.WriteLineSuccess("\nPages after crawling (timings):");
crawlerResults.ToList().ForEach(Console.WriteLine);
ConsoleMessage.WriteLineSuccess("Pages from sitemap.xml only (timings)");
pageLoadResultsForOnlyFromSiteMapLinks.ForEach(Console.WriteLine);

Console.WriteLine($"\n{finalReport}");