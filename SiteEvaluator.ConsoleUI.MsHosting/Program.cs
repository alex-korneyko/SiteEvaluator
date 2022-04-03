using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SiteEvaluator.Crawler;
using SiteEvaluator.Data.DataBaseContext;
using SiteEvaluator.Data.DataHandlers;
using SiteEvaluator.Data.Model;
using SiteEvaluator.DataLoader;
using SiteEvaluator.DataLoader.HttpLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Presentation;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.SiteMapExploring.Parser;

namespace SiteEvaluator.ConsoleUI.MsHosting
{
    class Program
    {
        static Task Main(string[] args)
        {
            return CreateBuilder(args)
                .Build()
                .RunAsync();
        }

        private static IHostBuilder CreateBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    //Services. Zero layer
                    services.AddScoped<IHttpLoaderService, HttpLoaderService>();
                    services.AddEfRepository<SiteEvaluatorDbContext>(_ => { });

                    //Services. First layer
                    services.AddScoped<IContentLoaderService, ContentLoaderService>();
                    services.AddScoped<IHtmlParseService, HtmlParseService>();
                    services.AddScoped<ISiteMapParseService, SiteMapParseService>();
                    services.AddScoped<IDataHandlerService, FileDataHandlerService>();
                    services.AddScoped<IDataHandlerService, DbDataHandlerService>();

                    //Services. Second layer
                    services.AddScoped<ISiteCrawler, SiteCrawler>();
                    services.AddScoped<ISiteMapExplorer, SiteMapExplorer>();
                    services.AddScoped<IReportService, ReportService>();

                    services.AddScoped<IConsoleView, ConsoleView>();

                    services.AddHostedService<HostedApplication>();
                });
        }
    }
}