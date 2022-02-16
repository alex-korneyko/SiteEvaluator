using SiteEvaluator.Crawler;
using SiteEvaluator.PageLoader;

var commandLineArgs = Environment.GetCommandLineArgs();

var baseUrl = "https://www.ukad-group.com/";

var siteCrawler = new SiteCrawler(new PageLoader(), settings =>
{
    settings.IncludeNofollowLinks = false;
});

var pageLoadResults = await siteCrawler.CrawlAsync(baseUrl);

Console.WriteLine("--------------");
foreach (var pageLoadResult in pageLoadResults)
{
    Console.WriteLine(pageLoadResult);
}
Console.WriteLine("--------------");
Console.WriteLine($"Total crawled pages: {pageLoadResults.Count}");