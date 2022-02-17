using System.Diagnostics;
using System.Net;

namespace SiteEvaluator.PageLoader;

public class HttpContentLoader : IHttpContentLoader
{
    public async Task<PageLoadResult> LoadContentAsync(string pageUrl)
    {
        var stopwatch = new Stopwatch();
        using var httpClient = new HttpClient();
        var pageLoadResult = new PageLoadResult(pageUrl);

        try
        {
            if (!pageUrl.StartsWith("http://") && !pageUrl.StartsWith("https://") || !pageUrl.Contains('.'))
                throw new ArgumentException($"Wrong URL: {pageUrl}");
            
            stopwatch.Start();
            var httpResponseMessage = await httpClient.GetAsync(pageUrl);
            stopwatch.Stop();
            
            pageLoadResult.PageLoadTime = stopwatch.ElapsedMilliseconds;

            await pageLoadResult.ApplyHttpResponse(httpResponseMessage);
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            pageLoadResult.IsSuccess = false;
            pageLoadResult.Exception = e;

            return pageLoadResult;
        }
        
        return pageLoadResult;
    }

    public async Task<PageLoadResult> LoadRobotsAsync(string hostUrl)
    {
        var robotsUrl = (hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl) + "/robots.txt";

        var loadRobotsResult = await LoadContentAsync(robotsUrl);
        
        return loadRobotsResult;
    }

    public async Task<PageLoadResult> LoadSiteMapAsync(string hostUrl)
    {
        var loadRobotsResult = await LoadRobotsAsync(hostUrl);

        if (!loadRobotsResult.IsSuccess || loadRobotsResult.HttpStatusCode != HttpStatusCode.OK)
            return loadRobotsResult;

        var indexOfSitemap = loadRobotsResult.Content.IndexOf("Sitemap:", StringComparison.InvariantCulture);
        if (indexOfSitemap == -1)
            return loadRobotsResult;

        var siteMapUrl = loadRobotsResult.Content.Substring(indexOfSitemap + "Sitemap:".Length).Trim();

        var siteMapLoadResult = await LoadContentAsync(siteMapUrl);

        return siteMapLoadResult;
    }
}