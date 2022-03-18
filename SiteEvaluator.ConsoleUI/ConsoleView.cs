using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteEvaluator.ConsoleUI.ConsoleXtend;
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
                ConsoleX.WriteLine.Warning($"Start crawling: {hostUrl}\n\t↓ ↓ ↓");
            
            IList<PageInfo> siteCrawlerResults = await _siteCrawler
                .CrawlAsync(hostUrl, settings => 
                    GetCrawlerSettings(logToConsole, includeNofollowLinks, settings));
            
            if (logToConsole) 
                ConsoleX.WriteLine.Success($"Crawling finished. Crawled: {siteCrawlerResults.Count} pages\n");

            await _reportService.AddCrawlerResultsAsync(hostUrl, siteCrawlerResults);
            
            if (logToConsole) 
                ConsoleX.WriteLine.Warning($"Start sitemap.xml exploring: {hostUrl}\nLoad content...\n\t↓ ↓ ↓");
            
            IList<PageInfo> siteMapExplorerResults = await _siteMapExplorer
                .ExploreAsync(hostUrl, settings => 
                    GetSiteMapExplorerSettings(logToConsole, settings, siteCrawlerResults));
            
            if (logToConsole) 
                ConsoleX.WriteLine.Success($"sitemap.xml explored. Founded: {siteMapExplorerResults.Count} links\n");

            await _reportService.AddSiteMapExplorerResultsAsync(hostUrl, siteMapExplorerResults);

            return siteCrawlerResults.Count + siteMapExplorerResults.Count;
        }
        
        public async Task ShowUniqLinksInSiteMapAsync(string hostUrl)
        {
            ConsoleX.WriteLine.Warning("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");

            var links = (await _reportService.GetUniqInSiteMapResults(hostUrl))
                .Select(result => result.Url)
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
                .Select(result => result.Url)
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
            
            PagesInfoTableConsoleComponent(compositeReport.ToList());

            ConsoleX.WriteLine.Info();
            
            ConsoleX.WriteLine.Success("Urls(html documents) found after crawling a website: " +
                                       $"{(await _reportService.GetCrawlerResultsAsync(hostUrl)).Count()}");
            
            ConsoleX.WriteLine.Success($"Urls found in sitemap: " +
                                       $"{(await _reportService.GetSiteMapResultsAsync(hostUrl)).Count()}");
        }

        private void PagesInfoTableConsoleComponent(IList<PageInfo> pages)
        {
            var maxUrlLength = pages.Max(page => page.Url.Length);

            foreach (var pageInfo in pages)
            {
                var sbUrl = new StringBuilder(pageInfo.Url);
                sbUrl.Append(' ').AppendJoin("", Enumerable.Repeat('.', maxUrlLength - pageInfo.Url.Length));

                ConsoleX.Write.Info($"Url: {sbUrl}");
                ConsoleX.Write.Info($"   Loading: {pageInfo.TotalLoadTime}ms");
                
                ConsoleX.Write.Info(pageInfo.TotalSize < (1000 * 1024)
                    ? $"   Size: {Math.Round((double)pageInfo.TotalSize / 1024, 2)}Kb"
                    : $"   Size: {Math.Round((double)pageInfo.TotalSize / 1024 / 1024, 2)}Mb");
                
                // ConsoleX.Write.Info($"   Level: {pageInfo.Level}");
                ConsoleX.Write.Info($"   Inner links: {pageInfo.InnerUrls.Count}");
                ConsoleX.Write.Info($"   Outer links: {pageInfo.OuterUrls.Count}");
                ConsoleX.WriteLine.Info($"   Images: {pageInfo.MediaUrls.Count}");
            }
        }
        
        private static void GetSiteMapExplorerSettings(bool logToConsole, ExploreSettings settings, IEnumerable<PageInfo> siteCrawlerResults)
        {
            settings.LoadContent = true;
            settings.LoadMedia = true;
            settings.UrlsForExcludeLoadContent
                .AddRange(siteCrawlerResults
                    .Select(result => result.Url));
            settings.ExploreHtmlLoadedEvent = logToConsole
                ? result => ConsoleX.WriteLine.Info($"Page: {result}")
                : null;
            settings.ExploreImageLoadedEvent = logToConsole
                ? result => ConsoleX.WriteLine.Comment($"\tImage: {result}")
                : null;
        }

        private static void GetCrawlerSettings(bool logToConsole, bool includeNofollowLinks, CrawlerSettings settings)
        {
            settings.IncludeNofollowLinks = includeNofollowLinks;
            settings.CrawlHtmlLoadedEvent = logToConsole
                ? result => ConsoleX.WriteLine.Info($"Page: {result}")
                : null;
            settings.LoadMedia = true;
            settings.CrawlImageLoadedEvent = logToConsole
                ? result => ConsoleX.WriteLine.Comment($"\tImage: {result}")
                : null;
        }

    }
}