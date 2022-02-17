using System.Text;
using SiteEvaluator.Crawler;
using SiteEvaluator.PageLoader;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExplorer;

var baseUrl = "https://www.ukad-group.com/";

var httpContentLoader = new HttpContentLoader();
ISiteCrawler siteCrawler = new SiteCrawler(httpContentLoader, settings =>
{
    settings.IncludeNofollowLinks = false;
    settings.LogToConsole = false;
    settings.PrintResult = false;
});

var crawlerResults = await siteCrawler.CrawlAsync(baseUrl);

var finalReport = new StringBuilder($"Urls(html documents) found after crawling a website: {crawlerResults.Count}");

Console.WriteLine($"Total crawled pages: {crawlerResults.Count}\n");

ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(httpContentLoader, settings =>
{
    settings.PrintResult = false;
});

var siteMapExploreResult = await siteMapExplorer.ExploreAsync(baseUrl);

var pageLoadResultsForOnlyFromSiteMapLinks = new List<PageLoadResult>();

if (siteMapExploreResult.IsSuccess)
{
    Console.WriteLine($"Total in sitemap.xml: {siteMapExploreResult.SiteMap.UrlSet?.Length}\n");

    var siteMapUrls = siteMapExploreResult.SiteMap.UrlSet!
        .Where(url => url.Loc != null)
        .Select(url => new PageLoadResult(url.Loc!))
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

ConsoleMessage.WriteLineSuccess("\nPages after crawling:");
crawlerResults.ToList().ForEach(Console.WriteLine);
ConsoleMessage.WriteLineSuccess("Pages from only sitemap.xml");
pageLoadResultsForOnlyFromSiteMapLinks.ForEach(Console.WriteLine);

Console.WriteLine($"\n{finalReport}");