using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Crawler;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;

namespace SiteEvaluator.ConsoleUI
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

            var hostUrl = GetHostUrl(args);

            IList<ContentLoadResult> siteCrawlerResults = await _siteCrawler
                .CrawlAsync(hostUrl, settings =>
                {
                    settings.IncludeNofollowLinks = false;
                    settings.LogToConsole = true;
                    settings.PrintResult = false;
                });

            report.AddCrawlerResults(siteCrawlerResults);

            IList<ContentLoadResult> siteMapExplorerResults = await _siteMapExplorer
                .ExploreAsync(hostUrl, settings =>
                {
                    settings.PrintResult = false;
                    settings.LoadContent = true;
                    settings.UrlsForExcludeLoadContent
                        .AddRange(siteCrawlerResults
                            .Select(result => result.PageUrl));
                });

            report.AddSiteMapExplorerResults(siteMapExplorerResults);

            await report.ReportDifferences();

            report.ReportSummary();
        }

        private static string GetHostUrl(string[] args)
        {
            var hostUrl = args.Length == 0
                ? ConsoleController.ReadLine.Warning("Please, enter host URL for evaluate: ")
                : args[0];

            if (hostUrl.Equals(""))
                hostUrl = "https://www.ukad-group.com/";

            return hostUrl;
        }
    }
}