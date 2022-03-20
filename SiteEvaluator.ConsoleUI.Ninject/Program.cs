using System.Threading.Tasks;
using Ninject;
using SiteEvaluator.Crawler;
using SiteEvaluator.Data;
using SiteEvaluator.DataLoader;
using SiteEvaluator.DataLoader.HttpLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.SiteMapExploring.Parser;

namespace SiteEvaluator.ConsoleUI.Ninject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var applicationBuilder = ConsoleApplication.CreateBuilder(args);

            var kernel = ConfigureDependencies();

            applicationBuilder.SetBootstrap(kernel.Get<IConsoleView>());

            await applicationBuilder.Build().StartAsync();
        }

        private static IKernel ConfigureDependencies()
        {
            IKernel kernel = new StandardKernel();

            //Services. Zero layer
            kernel.Bind<IHttpLoaderService>().To<HttpLoaderService>();
            
            //Services. First layer
            kernel.Bind<IContentLoaderService>().To<ContentLoaderService>();
            kernel.Bind<IHtmlParseService>().To<HtmlParseService>();
            kernel.Bind<ISiteMapParseService>().To<SiteMapParseService>();
            kernel.Bind<IDao<PageInfo>>().To<FileDao<PageInfo>>();
            
            //Services. Second layer
            kernel.Bind<ISiteCrawler>().To<SiteCrawler>();
            kernel.Bind<ISiteMapExplorer>().To<SiteMapExplorer>();
            kernel.Bind<IReportService>().To<ReportService>();
            
            //View
            kernel.Bind<IConsoleView>().To<ConsoleView>();

            return kernel;
        }
    }
}