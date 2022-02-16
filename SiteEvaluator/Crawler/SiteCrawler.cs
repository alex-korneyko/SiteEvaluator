using System.Net;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Tags;
using SiteEvaluator.PageLoader;
using SiteEvaluator.Presentation;

namespace SiteEvaluator.Crawler;

public class SiteCrawler : ISiteCrawler
{
    private readonly IPageLoader _pageLoader;
    private readonly List<PageLoadResult> _result = new();
    private readonly CrawlerSettings _settings = new();

    public SiteCrawler(IPageLoader pageLoader)
    {
        _pageLoader = pageLoader;
    }

    public SiteCrawler(IPageLoader pageLoader, Action<CrawlerSettings> crawlerSettings) : this(pageLoader)
    {
        crawlerSettings.Invoke(_settings);
    }
    
    public async Task<IList<PageLoadResult>> CrawlAsync(string hostUrl)
    {
        hostUrl = hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl;
        
        var pageLoadResult = await _pageLoader.LoadPageAsync(hostUrl);
        _result.Add(pageLoadResult);

        var pageBody = string.Empty;
        
        if (pageLoadResult.HttpStatusCode == HttpStatusCode.OK)
        {
            pageBody = HtmlSerializer.GetBody(pageLoadResult.PageContent);
        }
        
        await ScanLinksAsync(pageBody, hostUrl);

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

            Console.Write($"Attempt to load : {aLinkTag.Href} ... ");

            var pageLoadResult = await _pageLoader.LoadPageAsync(fullUrl);

            _result.Add(pageLoadResult);

            if (!pageLoadResult.IsSuccess)
            {
                ConsoleMessage.WriteLineError($"Page loading unsuccessful - {fullUrl}");
                continue;
            }
            
            ConsoleMessage.WriteLineSuccess(pageLoadResult.ToString());

            await ScanLinksAsync(pageLoadResult.PageContent, hostUrl);
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
}