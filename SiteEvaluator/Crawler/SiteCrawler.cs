using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Nodes;

namespace SiteEvaluator.Crawler
{
    public class SiteCrawler : ISiteCrawler
    {
        private readonly IHttpContentLoaderService _httpContentLoaderService;
        private readonly IHtmlParseService _htmlParseService;
        private readonly List<ContentLoadResult> _result = new();
        private readonly CrawlerSettings _settings = new();

        public SiteCrawler(IHttpContentLoaderService httpContentLoaderService, IHtmlParseService htmlParseService)
        {
            _httpContentLoaderService = httpContentLoaderService;
            _htmlParseService = htmlParseService;
        }

        public async Task<IList<ContentLoadResult>> CrawlAsync(string hostUrl, Action<CrawlerSettings>? crawlerSettings = null)
        {
            crawlerSettings?.Invoke(_settings);

            hostUrl = hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl;

            ContentLoadResult pageLoadResult = await _httpContentLoaderService.LoadContentAsync(hostUrl);
            
            _result.Add(pageLoadResult);
            _settings.CrawlEvent?.Invoke(pageLoadResult);

            var pageBody = string.Empty;

            if (pageLoadResult.HttpStatusCode == HttpStatusCode.OK)
            {
                pageBody = _htmlParseService.ExtractBodyNode(pageLoadResult.Content);
            }

            await ScanLinksAsync(pageBody, hostUrl);

            return _result;
        }

        private async Task ScanLinksAsync(string pageBody, string hostUrl)
        {
            var allTagFullStrings = _htmlParseService.GetNodesAsStringsList<A>(pageBody);

            foreach (var tagFullString in allTagFullStrings)
            {
                var aLinkTag = _htmlParseService.DeserializeToNode<A>(tagFullString);

                if (aLinkTag == null)
                    continue;

                var fullUrl = GetFullUrl(aLinkTag, hostUrl);

                if (string.IsNullOrEmpty(fullUrl) || _result.Contains(new ContentLoadResult(fullUrl)))
                    continue;

                if (!_settings.IncludeNofollowLinks && aLinkTag.Rel == "nofollow")
                    continue;

                var pageLoadResult = await _httpContentLoaderService.LoadContentAsync(fullUrl);

                _result.Add(pageLoadResult);
                _settings.CrawlEvent?.Invoke(pageLoadResult);

                if (!pageLoadResult.IsSuccess)
                    continue;

                await ScanLinksAsync(pageLoadResult.Content, hostUrl);
            }
        }

        private string GetFullUrl(A aTag, string hostUrl)
        {
            if (aTag.Href != null)
            {
                aTag.Href = aTag.Href.EndsWith('/') ? aTag.Href : aTag.Href + "/";

                if (aTag.Href.StartsWith(hostUrl))
                {
                    return aTag.Href;
                }
            }

            if (aTag.Href == null 
                || aTag.Href.StartsWith("http") 
                || !aTag.Href.StartsWith('/')
                || aTag.Href is "#" or "\\")
            {
                return "";
            }

            return hostUrl + aTag.Href;
        }
    }
}