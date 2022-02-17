using System.Net;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Tags;
using SiteEvaluator.PageLoader;
using SiteEvaluator.Presentation;

namespace SiteEvaluator.Crawler;

public class SiteCrawler : ISiteCrawler
{
    private readonly IHttpContentLoader _httpContentLoader;
    private readonly List<PageLoadResult> _result = new();
    private readonly CrawlerSettings _settings = new();

    public SiteCrawler(IHttpContentLoader httpContentLoader)
    {
        _httpContentLoader = httpContentLoader;
    }

    public SiteCrawler(IHttpContentLoader httpContentLoader, Action<CrawlerSettings> crawlerSettings) : this(httpContentLoader)
    {
        crawlerSettings.Invoke(_settings);
    }
    
    public async Task<IList<PageLoadResult>> CrawlAsync(string hostUrl)
    {
        ConsoleMessage.WriteLineWarning("Start crawling...");
        
        hostUrl = hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl;
        
        var pageLoadResult = await _httpContentLoader.LoadContentAsync(hostUrl);
        _result.Add(pageLoadResult);

        var pageBody = string.Empty;
        
        if (pageLoadResult.HttpStatusCode == HttpStatusCode.OK)
        {
            pageBody = HtmlSerializer.GetBody(pageLoadResult.Content);
        }
        
        await ScanLinksAsync(pageBody, hostUrl);
        
        ConsoleMessage.WriteLineSuccess("Crawling finished!");

        if (_settings.PrintResult) PrintResult(_result);

        return _result;
    }

    private async Task ScanLinksAsync(string pageBody, string hostUrl)
    {
        var allTagFullStrings = HtmlSerializer.GetAllTagFullStrings<A>(pageBody);
        
        foreach (var tagFullString in allTagFullStrings)
        {
            var aLinkTag = HtmlSerializer.Deserialize<A>(tagFullString);

            if (aLinkTag == null) continue;
            
            var fullUrl = GetFullUrl(aLinkTag, hostUrl);

            if (string.IsNullOrEmpty(fullUrl) || _result.Contains(new PageLoadResult(fullUrl)))
            {
                continue;
            }
            
            if (!_settings.IncludeNofollowLinks && aLinkTag.Rel == "nofollow")
            {
                continue;
            }

            if (_settings.LogToConsole) Console.Write($"Attempt to load : {aLinkTag.Href} ... ");

            var pageLoadResult = await _httpContentLoader.LoadContentAsync(fullUrl);

            _result.Add(pageLoadResult);

            if (!pageLoadResult.IsSuccess)
            {
                ConsoleMessage.WriteLineError($"Page loading unsuccessful - {fullUrl}");
                continue;
            }
            
            if (_settings.LogToConsole) ConsoleMessage.WriteLineSuccess(pageLoadResult.ToString());

            await ScanLinksAsync(pageLoadResult.Content, hostUrl);
        }
    }

    private string GetFullUrl(A aTag, string hostUrl)
    {
        if (aTag.Href != null && aTag.Href.StartsWith(hostUrl))
        {
            return aTag.Href;
        }

        if (aTag.Href == null || aTag.Href.StartsWith("http") || !aTag.Href.StartsWith('/') || aTag.Href is "#" or "\\")
        {
            return "";
        }

        return hostUrl + aTag.Href;
    }
    
    private static void PrintResult(List<PageLoadResult> result)
    {
        ConsoleMessage.WriteLineSuccess("Crawling result:");
        foreach (var pageLoadResult in result)
        {
            Console.WriteLine(pageLoadResult);
        }
    }
}