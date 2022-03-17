using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.DataLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Nodes;

namespace SiteEvaluator.Crawler
{
    public class SiteCrawler : ISiteCrawler
    {
        private readonly IContentLoaderService _contentLoaderService;
        private readonly IHtmlParseService _htmlParseService;
        private readonly List<PageInfo> _result = new();
        private readonly CrawlerSettings _settings = new();

        public SiteCrawler(IContentLoaderService contentLoaderService, IHtmlParseService htmlParseService)
        {
            _contentLoaderService = contentLoaderService;
            _htmlParseService = htmlParseService;
        }

        public async Task<IList<PageInfo>> CrawlAsync(string hostUrl, Action<CrawlerSettings>? crawlerSettings = null)
        {
            crawlerSettings?.Invoke(_settings);

            hostUrl = hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl;

            StringLoadResult stringLoadResult = await _contentLoaderService.LoadHtmlAsync(hostUrl);
            
            var pageInfo = new PageInfo(stringLoadResult)
            {
                Level = 0
            };

            _result.Add(pageInfo);
            _settings.CrawlHtmlLoadedEvent?.Invoke(stringLoadResult);

            if (stringLoadResult.HttpStatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(pageInfo.Content))
            {
                await ScanLinksAsync(hostUrl, pageInfo);
            }
            
            return _result;
        }

        private async Task ScanLinksAsync(string hostUrl, PageInfo pageInfo)
        {
            await ScanAndApplyMediaLinks(pageInfo);
            ScanAndApplyALinks(hostUrl, pageInfo);
            
            var pageBody = _htmlParseService.ExtractBodyNode(pageInfo.Content);
            
            var aNodes = _htmlParseService.GetAllNodes<A>(pageBody);

            foreach (var aNode in aNodes)
            {
                if (!_settings.IncludeNofollowLinks && aNode.Rel == "nofollow")
                    continue;
                
                var fullUrl = GetFullUrl(aNode, hostUrl);
                if (string.IsNullOrEmpty(fullUrl))
                    continue;

                if (!fullUrl.StartsWith(hostUrl))
                {
                    pageInfo.OuterUrls.Add(fullUrl);
                    continue;
                }

                if (!CompareUrls(pageInfo.Url, fullUrl))
                    pageInfo.InnerUrls.Add(fullUrl);

                var alreadyCrawledPage = _result.FirstOrDefault(page => CompareUrls(page.Url, fullUrl));
                if (alreadyCrawledPage != null)
                {
                    alreadyCrawledPage.Level = alreadyCrawledPage.Level > (pageInfo.Level + 1) 
                        ? pageInfo.Level + 1 
                        : alreadyCrawledPage.Level;
                    continue;
                }

                var htmlLoadResult = await _contentLoaderService.LoadHtmlAsync(fullUrl);
                if (htmlLoadResult.HttpStatusCode != HttpStatusCode.OK || !htmlLoadResult.ContentType.Contains("text/html"))
                    continue;
                
                var newPageInfo = new PageInfo(htmlLoadResult, ++pageInfo.Level);

                _result.Add(newPageInfo);
                _settings.CrawlHtmlLoadedEvent?.Invoke(htmlLoadResult);

                if (!htmlLoadResult.IsSuccess)
                    continue;

                await ScanLinksAsync(hostUrl, newPageInfo);
            }
        }

        private bool CompareUrls(string url1, string url2)
        {
            url1 = url1.EndsWith('/') ? url1[..^1] : url1;
            url2 = url2.EndsWith('/') ? url2[..^1] : url2;

            return url1.Equals(url2);
        }

        private string GetFullUrl(A aNode, string hostUrl)
        {
            if (aNode.Href is null or "#" or "\\")
                return string.Empty;

            if (aNode.Href.StartsWith("http"))
                return aNode.Href;

            if (aNode.Href.StartsWith('#') || aNode.Href.Contains(':'))
                return string.Empty;

            return hostUrl + (aNode.Href == "/" ? "" : aNode.Href);
        }

        private async Task  ScanAndApplyMediaLinks(PageInfo pageInfo)
        {
            var allImgNodes = _htmlParseService.GetAllNodes<Img>(pageInfo.Content);
            var tasks = new List<Task>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            foreach (var imgNode in allImgNodes)
            {
                if (string.IsNullOrEmpty(imgNode.Src))
                    continue;
            
                pageInfo.MediaUrls.Add(imgNode.Src);

                var loadImageTask = _contentLoaderService
                    .LoadImageAsync(imgNode.Src)
                    .ContinueWith(imageLoadResult =>
                    {
                        if (imageLoadResult.Result.IsSuccess)
                            pageInfo.TotalSize += imageLoadResult.Result.Size;

                        _settings.CrawlImageLoadedEvent?.Invoke(imageLoadResult.Result);
                    });
                
                tasks.Add(loadImageTask);
            }

            await Task.WhenAll(tasks);
            
            stopwatch.Stop();
            pageInfo.TotalLoadTime += stopwatch.ElapsedMilliseconds;
        }

        private void ScanAndApplyALinks(string hostUrl, PageInfo pageInfo)
        {
            
        }
    }
}