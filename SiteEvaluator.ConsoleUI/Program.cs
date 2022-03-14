using System.Net.Http;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Crawler;
using SiteEvaluator.Data;
using SiteEvaluator.Html;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.Xml;

namespace SiteEvaluator.ConsoleUI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Services. First layer
            IHttpContentLoaderService httpContentLoaderService = new HttpContentLoaderService(new HttpClient());
            IHtmlParseService htmlParseService = new HtmlParseService();
            ISiteMapParseService siteMapParseService = new SiteMapParseService();
            IDao<ContentLoadResult> dao = new FileDao<ContentLoadResult>();

            //Services. Second layer
            ISiteCrawler siteCrawler = new SiteCrawler(httpContentLoaderService, htmlParseService);
            ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(httpContentLoaderService, siteMapParseService);
            IReportService reportService = new ReportService(dao);

            var application = new Application(siteMapExplorer, siteCrawler, reportService);
            
            await application.StartAsync(args);
        }
    }
}