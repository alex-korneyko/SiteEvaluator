using System;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.DataLoader.HttpLoader;

namespace SiteEvaluator.DataLoader
{
    public class ContentLoaderService : IContentLoaderService
    {
        private readonly IHttpLoaderService _httpLoaderService;

        public ContentLoaderService(IHttpLoaderService httpLoaderService)
        {
            _httpLoaderService = httpLoaderService;
        }

        // public async Task<ContentLoadResult> LoadContentAsync(string pageUrl)
        // {
        //     var stopwatch = new Stopwatch();
        //     var pageLoadResult = new ContentLoadResult(pageUrl);
        //
        //     try
        //     {
        //         if (!pageUrl.StartsWith("http://") && !pageUrl.StartsWith("https://"))
        //             throw new ArgumentException($"Wrong URL: {pageUrl}");
        //
        //         stopwatch.Start();
        //         var httpResponseMessage = await _httpClient.GetAsync(pageUrl);
        //         stopwatch.Stop();
        //
        //         pageLoadResult.PageLoadTime = stopwatch.ElapsedMilliseconds;
        //
        //         await pageLoadResult.ApplyHttpResponseAsync(httpResponseMessage);
        //     }
        //     catch (Exception e)
        //     {
        //         stopwatch.Stop();
        //         pageLoadResult.IsSuccess = false;
        //         pageLoadResult.Exception = e;
        //
        //         return pageLoadResult;
        //     }
        //
        //     return pageLoadResult;
        // }
        //
        public async Task<StringLoadResult> LoadRobotsAsync(string hostUrl)
        {
            var robotsUrl = (hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl) + "/robots.txt";
        
            var httpExtendedResponse = await _httpLoaderService.LoadAsync(robotsUrl);

            var stringLoadResult = new StringLoadResult(hostUrl);
            await stringLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

            return stringLoadResult;
        }
        
        public async Task<StringLoadResult> LoadSiteMapAsync(string hostUrl)
        {
            var loadRobotsResult = await LoadRobotsAsync(hostUrl);

            if (!loadRobotsResult.IsSuccess
                || loadRobotsResult.HttpStatusCode != HttpStatusCode.OK
                || loadRobotsResult.Content == null)
            {
                return loadRobotsResult;
            }

            var siteMapUrl = GetSiteMapUrl(loadRobotsResult.Content);
            
            if (string.IsNullOrEmpty(siteMapUrl))
            {
                return new StringLoadResult(hostUrl)
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                };
            }
        
            var siteMapLoadResult = await _httpLoaderService.LoadAsync(siteMapUrl);

            var stringLoadResult = new StringLoadResult(hostUrl);
            await stringLoadResult.ApplyHttpResponseAsync(siteMapLoadResult);

            return stringLoadResult;
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

        public async Task<StringLoadResult> LoadHtmlAsync(string requestUri)
        {
            var httpExtendedResponse = await _httpLoaderService.LoadAsync(requestUri);

            var htmlLoadResult = new StringLoadResult(requestUri);
            
            await htmlLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

            return htmlLoadResult;
        }

        public async Task<ImageLoadResult> LoadImageAsync(string requestUri)
        {
            var httpExtendedResponse = await _httpLoaderService.LoadAsync(requestUri);

            var imageLoadResult = new ImageLoadResult(requestUri);

            await imageLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

            return imageLoadResult;
        }
        
        public async Task<FileLoadResult> LoadFile(string requestUri)
        {
            var httpExtendedResponse = await _httpLoaderService.LoadAsync(requestUri);

            var fileLoadResult = new FileLoadResult(requestUri);

            await fileLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

            return fileLoadResult;
        }
    }
}