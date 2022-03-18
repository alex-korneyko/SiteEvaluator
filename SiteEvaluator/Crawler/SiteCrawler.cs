using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.Common;
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

            var hostUri = new Uri(hostUrl);

            StringLoadResult stringLoadResult = await _contentLoaderService.LoadHtmlAsync(hostUri);
            
            var pageInfo = new PageInfo(stringLoadResult)
            {
                Level = 0
            };

            _result.Add(pageInfo);
            _settings.CrawlHtmlLoadedEvent?.Invoke(stringLoadResult);

            if (stringLoadResult.HttpStatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(pageInfo.Content))
            {
                await ScanLinksAsync(hostUri, pageInfo);
            }
            
            return _result;
        }

        private async Task ScanLinksAsync(Uri hostUri, PageInfo pageInfo)
        {
            var allImgNodes = _htmlParseService.GetAllNodes<Img>(pageInfo.Content);
            await _contentLoaderService.ScanAndApplyMediaLinks(pageInfo, allImgNodes, _settings.LoadMedia, _settings.CrawlImageLoadedEvent);

            var aNodes = _htmlParseService.GetAllNodes<A>(pageInfo.Content);

            pageInfo.OuterUrls = Utils.FilterOuterLinksNodes(aNodes, hostUri)
                .Select(aNode => aNode.Href)
                .Where(url => url != null)
                .ToList()!;

            var innerUrlANodes = Utils.FilterInnerLinkNodes(aNodes, hostUri);

            foreach (var aNode in innerUrlANodes)
            {
                if (!_settings.IncludeNofollowLinks && aNode.Rel == "nofollow")
                    continue;

                var currentNodeFullUri = new Uri(hostUri, aNode.Href);

                if (!hostUri.AbsoluteUri.Equals(currentNodeFullUri.AbsoluteUri))
                    pageInfo.InnerUrls.Add(currentNodeFullUri.AbsolutePath);

                var alreadyCrawledPage = _result.FirstOrDefault(page => page.Url.Equals(currentNodeFullUri.AbsoluteUri));
                if (alreadyCrawledPage != null)
                    continue;

                var htmlLoadResult = await _contentLoaderService.LoadHtmlAsync(currentNodeFullUri);
                if (htmlLoadResult.HttpStatusCode != HttpStatusCode.OK || !htmlLoadResult.ContentType.Contains("text/html"))
                    continue;
                
                var newPageInfo = new PageInfo(htmlLoadResult, ++pageInfo.Level);

                _result.Add(newPageInfo);
                _settings.CrawlHtmlLoadedEvent?.Invoke(htmlLoadResult);

                if (!htmlLoadResult.IsSuccess)
                    continue;

                await ScanLinksAsync(hostUri, newPageInfo);
            }
        }
    }
}