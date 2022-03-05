using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.ContentLoader
{
    public class HttpContentLoader : IHttpContentLoader
    {
        private readonly HttpClient _httpClient;

        public HttpContentLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ContentLoadResult> LoadContentAsync(string pageUrl)
        {
            var stopwatch = new Stopwatch();
            var pageLoadResult = new ContentLoadResult(pageUrl);

            try
            {
                if (!pageUrl.StartsWith("http://") && !pageUrl.StartsWith("https://") || !pageUrl.Contains('.'))
                    throw new ArgumentException($"Wrong URL: {pageUrl}");

                stopwatch.Start();
                var httpResponseMessage = await _httpClient.GetAsync(pageUrl);
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

            var indexOfSitemap = loadRobotsResult.Content.IndexOf("Sitemap:", StringComparison.InvariantCulture);
            if (indexOfSitemap == -1)
            {
                return new ContentLoadResult(hostUrl)
                {
                    HttpStatusCode = HttpStatusCode.NotFound
                };
            }

            var siteMapUrl = loadRobotsResult.Content.Substring(indexOfSitemap + "Sitemap:".Length).Trim();

            var siteMapLoadResult = await LoadContentAsync(siteMapUrl);

            return siteMapLoadResult;
        }
    }
}