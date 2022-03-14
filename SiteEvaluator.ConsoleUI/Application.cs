using System.Threading.Tasks;
using SiteEvaluator.ConsoleUI.ConsoleXtend;
using SiteEvaluator.Crawler;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;

namespace SiteEvaluator.ConsoleUI
{
    public class Application
    {
        private readonly ConsoleView _consoleView;

        public Application(ISiteMapExplorer siteMapExplorer, ISiteCrawler siteCrawler, IReportService reportService)
        {
            _consoleView = new ConsoleView(reportService, siteCrawler, siteMapExplorer);
        }

        public async Task StartAsync(params string[] args)
        {
            var hostUrl = GetHostUrl(args);
            
            await _consoleView.ScanHostAsync(hostUrl, true);

            await _consoleView.ShowUniqLinksInSiteMapAsync(hostUrl);

            await _consoleView.ShowUniqLinksByCrawlingAsync(hostUrl);

            await _consoleView.ShowCompositeReportAsync(hostUrl);

            ConsoleX.ReadLine.Note("Job completed. Press ENTER for exit...");
        }

        private static string GetHostUrl(string[] args)
        {
            var hostUrl = args.Length == 0
                ? ConsoleX.ReadLine.Warning("Please, enter host URL for evaluate: ")
                : args[0];

            if (hostUrl.Equals(""))
                hostUrl = "https://www.ukad-group.com/";

            return hostUrl;
        }
    }
}