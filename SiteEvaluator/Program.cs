using SiteEvaluator.Crawler;
using SiteEvaluator.PageLoader;

var commandLineArgs = Environment.GetCommandLineArgs();

var baseUrl = "https://www.ukad-group.com/";

// var siteCrawler = new SiteCrawler(new HttpContentLoader(), settings =>
// {
//     settings.IncludeNofollowLinks = false;
// });
//
// var pageLoadResults = await siteCrawler.CrawlAsync(baseUrl);
//
// Console.WriteLine("--------------");
// foreach (var pageLoadResult in pageLoadResults)
// {
//     Console.WriteLine(pageLoadResult);
// }
// Console.WriteLine("--------------");
// Console.WriteLine($"Total crawled pages: {pageLoadResults.Count}");

IHttpContentLoader httpContentLoader = new HttpContentLoader();
var contentLoadResult = await httpContentLoader.LoadSiteMapAsync(baseUrl);

if (contentLoadResult.IsSuccess)
{
    Console.WriteLine($"Status: {contentLoadResult.HttpStatusCode}");
    Console.WriteLine("sitemap.xml:");
    Console.WriteLine(contentLoadResult.Content);
}
else
{
    Console.WriteLine($"Exception: {contentLoadResult.Exception?.Message}");
}