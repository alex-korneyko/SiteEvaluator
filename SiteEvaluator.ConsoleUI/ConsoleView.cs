using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteEvaluator.ConsoleUI.ConsoleXtend;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Crawler;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;

namespace SiteEvaluator.ConsoleUI
{
    public class ConsoleView
    {
        private readonly IReportService _reportService;
        private readonly ISiteCrawler _siteCrawler;
        private readonly ISiteMapExplorer _siteMapExplorer;

        public ConsoleView(IReportService reportService, ISiteCrawler siteCrawler, ISiteMapExplorer siteMapExplorer)
        {
            _reportService = reportService;
            _siteCrawler = siteCrawler;
            _siteMapExplorer = siteMapExplorer;
        }
        
        public async Task<int> ScanHostAsync(string hostUrl, bool logToConsole = false, bool includeNofollowLinks = false)
        {
            if (logToConsole) 
                ConsoleX.WriteLine.Warning($"Start crawling: {hostUrl}\n\t↓↓↓");
            
            IList<ContentLoadResult> siteCrawlerResults = await _siteCrawler
                .CrawlAsync(hostUrl, settings =>
                {
                    settings.IncludeNofollowLinks = includeNofollowLinks;
                    settings.CrawlEvent = logToConsole 
                        ? result => ConsoleX.WriteLine.Comment(result.ToString())
                        : null;
                });
            
            if (logToConsole) 
                ConsoleX.WriteLine.Success($"Crawling finished. Crawled: {siteCrawlerResults.Count} pages\n");

            await _reportService.AddCrawlerResultsAsync(hostUrl, siteCrawlerResults);
            
            if (logToConsole) 
                ConsoleX.WriteLine.Warning($"Start sitemap.xml exploring: {hostUrl}\nLoad content...\n\t↓↓↓");
            
            IList<ContentLoadResult> siteMapExplorerResults = await _siteMapExplorer
                .ExploreAsync(hostUrl, settings =>
                {
                    settings.LoadContent = true;
                    settings.UrlsForExcludeLoadContent
                        .AddRange(siteCrawlerResults
                            .Select(result => result.PageUrl));
                    settings.ExploreEvent = logToConsole 
                        ? result => ConsoleX.WriteLine.Comment(result.ToString())
                        : null;
                });
            
            if (logToConsole) 
                ConsoleX.WriteLine.Success($"sitemap.xml explored. Founded: {siteMapExplorerResults.Count} links\n");

            await _reportService.AddSiteMapExplorerResultsAsync(hostUrl, siteMapExplorerResults);

            return siteCrawlerResults.Count + siteMapExplorerResults.Count;
        }
        
        public async Task ShowUniqLinksInSiteMapAsync(string hostUrl)
        {
            ConsoleX.WriteLine.Warning("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");

            var links = (await _reportService.GetUniqInSiteMapResults(hostUrl))
                .Select(result => result.PageUrl)
                .ToList();
            
            foreach (var link in links)
            {
                ConsoleX.WriteLine.Info(link);
            }
            
            ConsoleX.WriteLine.Success($"Total: {links.Count()}\n");
        }
        
        public async Task ShowUniqLinksByCrawlingAsync(string hostUrl)
        {
            ConsoleX.WriteLine.Warning("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");

            var links = (await _reportService.GetUniqCrawlerResults(hostUrl))
                .Select(result => result.PageUrl)
                .ToList();
            
            foreach (var link in links)
            {
                ConsoleX.WriteLine.Info(link);
            }
            
            ConsoleX.WriteLine.Success($"Total: {links.Count()}\n");
        }

        public async Task ShowCompositeReportAsync(string hostUrl)
        {
            var compositeReport = await _reportService.GetCompositeReportAsync(hostUrl);

            ConsoleX.WriteLine.Success("Composite report sorted by timings:");
            
            foreach (var contentLoadResult in compositeReport)
            {
                ConsoleX.WriteLine.Info(contentLoadResult.ToString());
            }
            
            ConsoleX.WriteLine.Info();
            
            ConsoleX.WriteLine.Success("Urls(html documents) found after crawling a website: " +
                                       $"{(await _reportService.GetCrawlerResultsAsync(hostUrl)).Count()}");
            
            ConsoleX.WriteLine.Success($"Urls found in sitemap: " +
                                       $"{(await _reportService.GetSiteMapResultsAsync(hostUrl)).Count()}");
        }
    }
}