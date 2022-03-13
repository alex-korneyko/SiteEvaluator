using System.Net.Http;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Crawler;
using SiteEvaluator.Html;
using SiteEvaluator.SiteMapExploring;

namespace SiteEvaluator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var httpContentLoader = new HttpContentLoaderService(new HttpClient());
            var htmlParseService = new HtmlParseService();

            ISiteCrawler siteCrawler = new SiteCrawler(httpContentLoader, htmlParseService);

            ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(httpContentLoader, settings =>
            {
                settings.PrintResult = false;
            });

            var application = new Application(siteMapExplorer, siteCrawler);
            await application.StartAsync(args);
        }
    }
}