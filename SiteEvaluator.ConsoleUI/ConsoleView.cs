using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteEvaluator.ConsoleUI.ConsoleXtend;
using SiteEvaluator.Crawler;
using SiteEvaluator.Data.Model;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;

namespace SiteEvaluator.ConsoleUI
{
    public class ConsoleView : IConsoleView
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
        
        public async Task<TargetHost> ScanHostAsync(Uri hostUri, bool logToConsole = false, bool includeNofollowLinks = false)
        {
            if (logToConsole) 
                ConsoleX.WriteLine.Warning($"Start crawling: {hostUri.Host}\n\t↓ ↓ ↓");

            var crawledHost = await _siteCrawler
                .CrawlAsync(hostUri, settings => 
                    GetCrawlerSettings(logToConsole, includeNofollowLinks, settings));

            if (logToConsole)
                PrintCount(crawledHost, ScannerType.SiteCrawler);

            if (logToConsole) 
                ConsoleX.WriteLine.Warning($"Start sitemap.xml exploring: {hostUri.Host}\nLoad content...\n\t↓ ↓ ↓");
            
            var siteMapScannedHost = await _siteMapExplorer
                .ExploreAsync(hostUri, settings => 
                    GetSiteMapExplorerSettings(logToConsole, settings, crawledHost.PageInfos));

            if (logToConsole)
                PrintCount(siteMapScannedHost, ScannerType.SiteMap);
            
            return siteMapScannedHost;
        }

        public async Task ShowUniqLinksInSiteMapAsync(Uri hostUri)
        {
            ConsoleX.WriteLine.Warning("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");

            var links = (await _reportService.GetUniqInSiteMapResults(hostUri))
                .Select(result => result.Url)
                .ToList();
            
            foreach (var link in links)
            {
                ConsoleX.WriteLine.Info(link);
            }
            
            ConsoleX.WriteLine.Success($"Total: {links.Count()}\n");
        }
        
        public async Task ShowUniqLinksByCrawlingAsync(Uri hostUri)
        {
            ConsoleX.WriteLine.Warning("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");

            var links = (await _reportService.GetUniqCrawlerResults(hostUri))
                .Select(result => result.Url)
                .ToList();
            
            foreach (var link in links)
            {
                ConsoleX.WriteLine.Info(link);
            }
            
            ConsoleX.WriteLine.Success($"Total: {links.Count()}\n");
        }

        public async Task ShowCompositeReportAsync(Uri hostUri)
        {
            var compositeReport = await _reportService.GetCompositeReportAsync(hostUri);

            ConsoleX.WriteLine.Success("Composite report sorted by timings:");
            
            PagesInfoTableConsoleComponent(compositeReport.ToList());

            ConsoleX.WriteLine.Info();
            
            ConsoleX.WriteLine.Success("Urls(html documents) found after crawling a website: " +
                                       $"{(await _reportService.GetCrawlerResultsAsync(hostUri)).Count()}");
            
            ConsoleX.WriteLine.Success($"Urls found in sitemap: " +
                                       $"{(await _reportService.GetSiteMapResultsAsync(hostUri)).Count()}");
        }
        
        private static void PrintCount(TargetHost crawledHost, ScannerType scannerType)
        {
            var count = crawledHost.PageInfos
                .Where(page => page.ScannerType == scannerType)
                .ToList().Count;

            ConsoleX.WriteLine.Success($"Handled: {count} pages by {scannerType.ToString()}\n");
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
                ConsoleX.Write.Info($"   Inner links: {pageInfo.PageInfoUrls.Count(url => url.PageInfoUrlType == PageInfoUrlType.Inner)}");
                ConsoleX.Write.Info($"   Outer links: {pageInfo.PageInfoUrls.Count(url => url.PageInfoUrlType == PageInfoUrlType.Outer)}");
                ConsoleX.WriteLine.Info($"   Images: {pageInfo.PageInfoUrls.Count(url => url.PageInfoUrlType == PageInfoUrlType.Media)}");
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