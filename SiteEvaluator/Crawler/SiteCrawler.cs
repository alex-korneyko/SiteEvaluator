using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, PageInfo> _result = new();
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
            var pageInfo = new PageInfo(stringLoadResult);

            _result.Add(hostUrl, pageInfo);
            _settings.CrawlHtmlLoadedEvent?.Invoke(stringLoadResult);

            // var pageBody = string.Empty;

            if (stringLoadResult.HttpStatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(pageInfo.Content))
            {
                await ScanLinksAsync(hostUrl, pageInfo, 0);
            }

            // await ScanLinksAsync(pageInfo, 0);

            return _result.Values.ToList();
        }

        private async Task ScanLinksAsync(string hostUrl, PageInfo pageInfo, int level)
        {
            await ScanMedia(pageInfo);
            
            var pageBody = _htmlParseService.ExtractBodyNode(pageInfo.Content);
            
            var allTagFullStrings = _htmlParseService.GetNodesAsStringsList<A>(pageBody);

            foreach (var tagFullString in allTagFullStrings)
            {
                var aNode = _htmlParseService.DeserializeToNode<A>(tagFullString);

                if (aNode == null)
                    continue;

                if (!_settings.IncludeNofollowLinks && aNode.Rel == "nofollow")
                    continue;
                
                var fullUrl = GetFullUrl(aNode, hostUrl);
                if (string.IsNullOrEmpty(fullUrl))
                    continue;

                if (!fullUrl.StartsWith(hostUrl))
                {
                    pageInfo.ExternalUrls.Add(fullUrl);
                    continue;
                }
                
                pageInfo.InnerUrls.Add(fullUrl);

                if (_result.ContainsKey(fullUrl))
                    continue;

                var htmlLoadResult = await _contentLoaderService.LoadHtmlAsync(fullUrl);
                var newPageInfo = new PageInfo(htmlLoadResult, level);

                _result.Add(fullUrl, newPageInfo);
                _settings.CrawlHtmlLoadedEvent?.Invoke(htmlLoadResult);

                if (!htmlLoadResult.IsSuccess)
                    continue;

                await ScanLinksAsync(hostUrl, newPageInfo, ++level);
            }
        }

        private string GetFullUrl(A aNode, string hostUrl)
        {
            if (aNode.Href is null or "#" or "\\")
                return string.Empty;

            if (aNode.Href.Contains(':'))
                return string.Empty;

            aNode.Href = aNode.Href.EndsWith('/') ? aNode.Href : aNode.Href + "/";

            if (aNode.Href.StartsWith("http"))
            {
                return aNode.Href;
            }

            return hostUrl + aNode.Href;
        }

        private async Task ScanMedia(PageInfo pageInfo)
        {
            var imgNodeStrings = _htmlParseService.GetNodesAsStringsList<Img>(pageInfo.Content);

            foreach (var imgNodeString in imgNodeStrings)
            {
                var imgNode = _htmlParseService.DeserializeToNode<Img>(imgNodeString);
                if (imgNode == null || string.IsNullOrEmpty(imgNode.Src))
                    continue;
                
                pageInfo.MediaUrls.Add(imgNode.Src);
                
                // var imageLoadResult = await _contentLoaderService.LoadImageAsync(imgNode.Src);
                var imageLoadResult = new ImageLoadResult(imgNode.Src);
                _settings.CrawlImageLoadedEvent?.Invoke(imageLoadResult);
                
                pageInfo.TotalLoadTime += imageLoadResult.ContentLoadTime;
                pageInfo.TotalSize += imageLoadResult.Size;
            }
        } 
    }
}