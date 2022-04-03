using System.Threading.Tasks;
using Autofac;
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

namespace SiteEvaluator.ConsoleUI.Autofac
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var applicationBuilder = ConsoleApplication.CreateBuilder(args);

            var container = ConfigureDependencies();

            applicationBuilder.SetBootstrap(container.Resolve<IConsoleView>());

            await applicationBuilder.Build().StartAsync();
        }
        
        private static IContainer ConfigureDependencies()
        {
            var containerBuilder = new ContainerBuilder();

            //Services. Zero layer
            containerBuilder.RegisterType<HttpLoaderService>().As<IHttpLoaderService>().SingleInstance();
            
            //Services. First layer
            containerBuilder.RegisterType<ContentLoaderService>().As<IContentLoaderService>().SingleInstance();
            containerBuilder.RegisterType<HtmlParseService>().As<IHtmlParseService>().SingleInstance();
            containerBuilder.RegisterType<SiteMapParseService>().As<ISiteMapParseService>().SingleInstance();
            containerBuilder.RegisterType<FileDataHandlerService>().As<IDataHandlerService>().SingleInstance();
            
            //Services. Second layer
            containerBuilder.RegisterType<SiteCrawler>().As<ISiteCrawler>().SingleInstance();
            containerBuilder.RegisterType<SiteMapExplorer>().As<ISiteMapExplorer>().SingleInstance();
            containerBuilder.RegisterType<ReportService>().As<IReportService>().SingleInstance();
            
            //View
            containerBuilder.RegisterType<ConsoleView>().As<IConsoleView>();

            return containerBuilder.Build();
        }
    }
}