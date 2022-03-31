using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;
using SiteEvaluator.DataLoader.HttpLoader;
using SiteEvaluator.Html.Nodes;

namespace SiteEvaluator.DataLoader
{
    public class ContentLoaderService : IContentLoaderService
    {
        private readonly IHttpLoaderService _httpLoaderService;

        public ContentLoaderService(IHttpLoaderService httpLoaderService)
        {
            _httpLoaderService = httpLoaderService;
        }
        
        public async Task<StringLoadResult> LoadRobotsAsync(Uri requestUri)
        {
            var robotsUri = new Uri(requestUri, "/robots.txt");
            var httpExtendedResponse = await _httpLoaderService.LoadAsync(robotsUri);

            var stringLoadResult = new StringLoadResult(robotsUri.AbsoluteUri);
            await stringLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

            return stringLoadResult;
        }
        
        public async Task<StringLoadResult> LoadSiteMapAsync(Uri requestUri)
        {
            var loadRobotsResult = await LoadRobotsAsync(requestUri);

            if (!loadRobotsResult.IsSuccess
                || loadRobotsResult.HttpStatusCode != HttpStatusCode.OK
                || loadRobotsResult.Content == null)
            {
                return loadRobotsResult;
            }

            var siteMapUrl = GetSiteMapUrl(loadRobotsResult.Content);
            
            if (string.IsNullOrEmpty(siteMapUrl))
            {
                return new StringLoadResult(requestUri.AbsoluteUri)
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                };
            }
        
            var siteMapLoadResult = await _httpLoaderService.LoadAsync(new Uri(siteMapUrl));

            var stringLoadResult = new StringLoadResult(siteMapUrl);
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

        public async Task<StringLoadResult> LoadHtmlAsync(Uri requestUri)
        {
            var httpExtendedResponse = await _httpLoaderService.LoadAsync(requestUri);

            var htmlLoadResult = new StringLoadResult(requestUri.ToString());
            
            await htmlLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

            return htmlLoadResult;
        }

        public async Task<ImageLoadResult> LoadImageAsync(Uri requestUri)
        {
            try
            {
                var httpExtendedResponse = await _httpLoaderService.LoadAsync(requestUri);

                var imageLoadResult = new ImageLoadResult(requestUri.AbsoluteUri);

                await imageLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

                return imageLoadResult;
            }
            catch (Exception e)
            {
                return new ImageLoadResult(requestUri.AbsoluteUri)
                {
                    IsSuccess = false,
                    Exception = e
                };
            }
        }
        
        public async Task<FileLoadResult> LoadFile(Uri requestUri)
        {
            var httpExtendedResponse = await _httpLoaderService.LoadAsync(requestUri);

            var fileLoadResult = new FileLoadResult(requestUri.AbsoluteUri);

            await fileLoadResult.ApplyHttpResponseAsync(httpExtendedResponse);

            return fileLoadResult;
        }
        
        public async Task ScanAndApplyMediaLinks(
            PageInfo pageInfo,
            IList<Img> allImgNodes,
            bool loadImgContent = false,
            Action<ImageLoadResult>? imageLoadedEvent = null)
        {
            var tasks = new List<Task>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            foreach (var imgNode in allImgNodes)
            {
                if (string.IsNullOrEmpty(imgNode.Src))
                    continue;
            
                pageInfo.PageInfoUrls.Add(new PageInfoUrl(imgNode.Src, PageInfoUrlType.Media));

                if (!loadImgContent) continue;

                var imgUri = FullUri(pageInfo, imgNode.Src);
                var loadImageTask = LoadImageAsync(imgUri)
                    .ContinueWith(imageLoadResult =>
                    {
                        if (imageLoadResult.Result.IsSuccess)
                            pageInfo.TotalSize += imageLoadResult.Result.Size;
                        
                        imageLoadedEvent?.Invoke(imageLoadResult.Result);
                    });
                
                tasks.Add(loadImageTask);
            }

            await Task.WhenAll(tasks);
            
            stopwatch.Stop();
            pageInfo.TotalLoadTime += stopwatch.ElapsedMilliseconds;
        }

        private Uri FullUri(PageInfo pageInfo, string relativeUrl)
        {
            return relativeUrl.Contains("//") 
                ? new Uri(relativeUrl, UriKind.Absolute) 
                : new Uri(new Uri(pageInfo.Url), relativeUrl);
        }
    }
}