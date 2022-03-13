using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Tags;
using SiteEvaluator.Presentation;

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
            
            ConsoleController.WriteLine.Warning("Start crawling...");

            hostUrl = hostUrl.EndsWith('/') ? hostUrl[..^1] : hostUrl;

            ContentLoadResult pageLoadResult = await _httpContentLoaderService.LoadContentAsync(hostUrl);
            
            if (_settings.LogToConsole) 
                PrintResultString(pageLoadResult);
            
            _result.Add(pageLoadResult);

            var pageBody = string.Empty;

            if (pageLoadResult.HttpStatusCode == HttpStatusCode.OK)
            {
                pageBody = _htmlParseService.ExtractBodyNode(pageLoadResult.Content);
            }

            await ScanLinksAsync(pageBody, hostUrl);

            ConsoleController.WriteLine.Success("Crawling finished!");

            if (_settings.PrintResult) 
                PrintResult(_result);

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

                if (!pageLoadResult.IsSuccess)
                {
                    ConsoleController.WriteLine.Error($"Page loading unsuccessful - {fullUrl}");
                    continue;
                }

                if (_settings.LogToConsole)
                    PrintResultString(pageLoadResult);

                await ScanLinksAsync(pageLoadResult.Content, hostUrl);
            }
        }

        private static void PrintResultString(ContentLoadResult contentLoadResult)
        {
            ConsoleController.Write.Info($"Page: {contentLoadResult.PageUrl}; Status: ");
            var httpStatusCode = (int)contentLoadResult.HttpStatusCode;
            switch (httpStatusCode)
            {
                case >= 200 and < 300:
                    ConsoleController.Write.Success($"{contentLoadResult.HttpStatusCode}({httpStatusCode})");
                    break;
                case >= 300 and < 400:
                    ConsoleController.Write.Warning($"{contentLoadResult.HttpStatusCode}({httpStatusCode})");
                    break;
                case >= 400:
                    ConsoleController.Write.Error($"{contentLoadResult.HttpStatusCode}({httpStatusCode})");
                    break;
                default:
                    ConsoleController.Write.Info($"{contentLoadResult.HttpStatusCode}({httpStatusCode})");
                    break;
            }
            
            ConsoleController.Write.Info($"; Content type: '{contentLoadResult.ContentType}'");
            ConsoleController.Write.Info($"; Load time: {contentLoadResult.PageLoadTime}ms");
            ConsoleController.WriteLine.Info($"; Size: {Math.Round((float)contentLoadResult.Content.Length / 1024, 2)}Kb");
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

        private static void PrintResult(List<ContentLoadResult> result)
        {
            ConsoleController.WriteLine.Success("Crawling result:");
            foreach (var pageLoadResult in result)
            {
                Console.WriteLine(pageLoadResult);
            }
        }
    }
}