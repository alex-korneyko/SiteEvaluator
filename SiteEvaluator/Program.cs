using System.Net.Http;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Crawler;
using SiteEvaluator.SiteMapExploring;

namespace SiteEvaluator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var httpContentLoader = new HttpContentLoader(new HttpClient());
            
            ISiteCrawler siteCrawler = new SiteCrawler(httpContentLoader, settings =>
            {
                settings.IncludeNofollowLinks = false;
                settings.LogToConsole = true;
                settings.PrintResult = false;
            });

            ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(httpContentLoader, settings =>
            {
                settings.PrintResult = false;
            });

            var application = new Application(siteMapExplorer, siteCrawler);
            await application.StartAsync(args);
        }
    }
}