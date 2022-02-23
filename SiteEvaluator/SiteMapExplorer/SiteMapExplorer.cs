using System;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.PageLoader;
using SiteEvaluator.Presentation;
using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExplorer
{
    public class SiteMapExplorer : ISiteMapExplorer
    {
        private readonly IHttpContentLoader _httpContentLoader;
        private readonly SiteMapExplorerSettings _settings = new();

        public SiteMapExplorer(IHttpContentLoader httpContentLoader)
        {
            _httpContentLoader = httpContentLoader;
        }

        public SiteMapExplorer(IHttpContentLoader httpContentLoader, Action<SiteMapExplorerSettings> settings) : this(
            httpContentLoader)
        {
            settings.Invoke(_settings);
        }

        public async Task<SiteMapExploreResult> ExploreAsync(string hostUrl)
        {
            ConsoleMessage.WriteLineWarning("Start sitemap.xml exploring...");

            var loadSiteMapResult = await _httpContentLoader.LoadSiteMapAsync(hostUrl);

            if (!loadSiteMapResult.IsSuccess)
                return new SiteMapExploreResult(loadSiteMapResult.Exception);
            if (loadSiteMapResult.HttpStatusCode != HttpStatusCode.OK)
            {
                var message = $"Http status code: {loadSiteMapResult.HttpStatusCode}; " +
                              $"Requested url: {loadSiteMapResult.PageUrl}";

                ConsoleMessage.WriteLineError(message);
                return new SiteMapExploreResult(message);
            }

            var siteMapString = loadSiteMapResult.Content;
            try
            {
                var siteMap = SiteMapSerializer.Deserialize(siteMapString);
                ConsoleMessage.WriteLineSuccess("sitemap.xml explored!");
                if (_settings.PrintResult) PrintToConsole(siteMap);

                return new SiteMapExploreResult(siteMap);
            }
            catch (Exception e)
            {
                ConsoleMessage.WriteLineError($"Exploring error: {e.Message}");
                return new SiteMapExploreResult(e);
            }
        }

        private static void PrintToConsole(SiteMap siteMap)
        {
            if (siteMap.UrlSet == null) return;

            ConsoleMessage.WriteLineSuccess("sitemap.xml exploring result: ");
            foreach (var url in siteMap.UrlSet)
            {
                Console.WriteLine($"loc: {url.Loc}\tlastmod: {url.LastMod}");
            }
        }
    }
}