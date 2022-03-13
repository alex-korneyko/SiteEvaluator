using System.Net.Http;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Crawler;
using SiteEvaluator.Html;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.Xml;

namespace SiteEvaluator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            //First level of services layer
            IHttpContentLoaderService httpContentLoader = new HttpContentLoaderService(new HttpClient());
            IHtmlParseService htmlParseService = new HtmlParseService();
            ISiteMapParseService siteMapParseService = new SiteMapParseService();

            //Second level of services layer
            ISiteCrawler siteCrawler = new SiteCrawler(httpContentLoader, htmlParseService);
            ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(httpContentLoader, siteMapParseService);

            var application = new Application(siteMapExplorer, siteCrawler);
            
            await application.StartAsync(args);
        }
    }
}