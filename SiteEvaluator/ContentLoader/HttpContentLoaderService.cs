using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.ContentLoader
{
    public class HttpContentLoaderService : IHttpContentLoaderService
    {
        private readonly HttpClient _httpClient;

        public HttpContentLoaderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ContentLoadResult> LoadContentAsync(string pageUrl)
        {
            var stopwatch = new Stopwatch();
            var pageLoadResult = new ContentLoadResult(pageUrl);

            try
            {
                if (!pageUrl.StartsWith("http://") && !pageUrl.StartsWith("https://"))
                    throw new ArgumentException($"Wrong URL: {pageUrl}");

                stopwatch.Start();
                var httpResponseMessage = await _httpClient.GetAsync(pageUrl);
                stopwatch.Stop();

                pageLoadResult.PageLoadTime = stopwatch.ElapsedMilliseconds;

                await pageLoadResult.ApplyHttpResponseAsync(httpResponseMessage);
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

        public async Task<ContentLoadResult> LoadRobotsAsync(string hostUrl)
        {
            var robotsUrl = (hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl) + "/robots.txt";

            var loadRobotsResult = await LoadContentAsync(robotsUrl);

            return loadRobotsResult;
        }

        public async Task<ContentLoadResult> LoadSiteMapAsync(string hostUrl)
        {
            var loadRobotsResult = await LoadRobotsAsync(hostUrl);

            if (!loadRobotsResult.IsSuccess || loadRobotsResult.HttpStatusCode != HttpStatusCode.OK)
                return loadRobotsResult;

            var siteMapUrl = GetSiteMapUrl(loadRobotsResult.Content);
            if (string.IsNullOrEmpty(siteMapUrl))
            {
                return new ContentLoadResult(hostUrl)
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                };
            }

            var siteMapLoadResult = await LoadContentAsync(siteMapUrl);

            return siteMapLoadResult;
        }

        private string GetSiteMapUrl(string robotsTxtContent)
        {
            const string sitemapWord = "Sitemap: ";
            
            var indexOfSitemap = robotsTxtContent.IndexOf(sitemapWord, StringComparison.InvariantCulture);
            if (indexOfSitemap == -1) 
                return "";

            var startIndexOfUrl = indexOfSitemap + sitemapWord.Length;

            var lengthOfUrl = robotsTxtContent.Substring(startIndexOfUrl).IndexOf('\n');
            
            lengthOfUrl = lengthOfUrl == -1
                ? robotsTxtContent.Substring(startIndexOfUrl).Length
                : lengthOfUrl;

            var siteMapUrl = robotsTxtContent
                .Substring(startIndexOfUrl, lengthOfUrl)
                .Trim();

            return siteMapUrl;
        }
    }
}