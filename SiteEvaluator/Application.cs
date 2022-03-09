using System.Threading.Tasks;
using SiteEvaluator.Crawler;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;

namespace SiteEvaluator
{
    public class Application
    {
        private readonly ISiteMapExplorer _siteMapExplorer;
        private readonly ISiteCrawler _siteCrawler;

        public Application(ISiteMapExplorer siteMapExplorer, ISiteCrawler siteCrawler)
        {
            _siteMapExplorer = siteMapExplorer;
            _siteCrawler = siteCrawler;
        }

        public async Task StartAsync(params string[] args)
        {
            var report = new Report();
            
            var hostUrl = ConsoleController.ReadLine.Warning("Please, enter host URL for evaluate: ");

            var siteCrawlerResults = await _siteCrawler.CrawlAsync(hostUrl);
            report.AddCrawlerResults(siteCrawlerResults);

            var siteMapExplorerResults = await _siteMapExplorer.ExploreAsync(hostUrl, false);
            report.AddSiteMapExplorerResults(siteMapExplorerResults);

            await report.ReportDifferences();

            report.ReportSummary();
        }
    }
}