using System.Threading.Tasks;
using SiteEvaluator.Crawler;
using SiteEvaluator.Data;
using SiteEvaluator.Data.DataHandlers;
using SiteEvaluator.Data.Model;
using SiteEvaluator.DataLoader;
using SiteEvaluator.DataLoader.HttpLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.SiteMapExploring.Parser;

namespace SiteEvaluator.ConsoleUI.Simple
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Services. Zero layer
            IHttpLoaderService httpLoaderService = new HttpLoaderService();
            
            //Services. First layer
            IContentLoaderService contentLoaderService = new ContentLoaderService(httpLoaderService);
            IHtmlParseService htmlParseService = new HtmlParseService();
            ISiteMapParseService siteMapParseService = new SiteMapParseService();
            IDataHandlerService<PageInfo> dataHandlerService = new FileDataHandlerService<PageInfo>();

            //Services. Second layer
            ISiteCrawler siteCrawler = new SiteCrawler(contentLoaderService, htmlParseService);
            ISiteMapExplorer siteMapExplorer = new SiteMapExplorer(contentLoaderService, siteMapParseService, htmlParseService);
            IReportService reportService = new ReportService(dataHandlerService);

            var applicationBuilder = ConsoleApplication.CreateBuilder(args);
            
            IConsoleView consoleView = new ConsoleView(reportService, siteCrawler, siteMapExplorer);
            
            applicationBuilder.SetBootstrap(consoleView);
            
            var application = applicationBuilder.Build();

            await application.StartAsync();
        }
    }
}