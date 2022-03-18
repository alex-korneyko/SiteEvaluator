using System.Net.Http;
using System.Threading.Tasks;
using SiteEvaluator.Crawler;
using SiteEvaluator.Data;
using SiteEvaluator.DataLoader;
using SiteEvaluator.DataLoader.HttpLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.SiteMapExploring.Parser;

namespace SiteEvaluator.ConsoleUI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Services. Zero layer
            IHttpLoaderService httpLoaderService = new HttpLoaderService(new HttpClient());
            
            //Services. First layer
            IContentLoaderService contentLoaderService = new ContentLoaderService(httpLoaderService);
            IHtmlParseService htmlParseService = new HtmlParseService();
            ISiteMapParseService siteMapParseService = new SiteMapParseService();
            IDao<PageInfo> dao = new FileDao<PageInfo>();

            //Services. Second layer
            ISiteCrawler siteCrawler = new SiteCrawler(contentLoaderService, htmlParseService);
            ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(contentLoaderService, siteMapParseService, htmlParseService);
            IReportService reportService = new ReportService(dao);

            var application = new Application(siteMapExplorer, siteCrawler, reportService);
            
            await application.StartAsync(args);
        }
    }
}