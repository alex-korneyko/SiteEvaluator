using System.Threading.Tasks;
using SiteEvaluator.ConsoleUI.ConsoleXtend;

namespace SiteEvaluator.ConsoleUI
{
    public class Application
    {
        private readonly IConsoleView _consoleView;
        private readonly string[] _args;

        protected internal Application(IConsoleView consoleView, params string[] args)
        {
            _consoleView = consoleView;
            _args = args;
        }

        public async Task StartAsync()
        {
            var hostUrl = GetHostUrl(_args);
            
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