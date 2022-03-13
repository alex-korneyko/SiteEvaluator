using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Presentation;
using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExploring
{
    public class SiteMapExplorer : ISiteMapExplorer
    {
        private readonly IHttpContentLoaderService _httpContentLoaderService;
        private readonly SiteMapExplorerSettings _settings = new();
        private readonly ExploreSettings _exploreSettings = new();

        public SiteMapExplorer(IHttpContentLoaderService httpContentLoaderService)
        {
            _httpContentLoaderService = httpContentLoaderService;
        }

        public SiteMapExplorer(IHttpContentLoaderService httpContentLoaderService, Action<SiteMapExplorerSettings> explorerSettings) 
            : this(httpContentLoaderService)
        {
            explorerSettings.Invoke(_settings);
        }

        public async Task<IList<ContentLoadResult>> ExploreAsync(string hostUrl, Action<ExploreSettings>? exploreSettings = null)
        {
            exploreSettings?.Invoke(_exploreSettings);

            ConsoleController.WriteLine.Warning("Start sitemap.xml exploring...");

            var loadSiteMapResult = await _httpContentLoaderService.LoadSiteMapAsync(hostUrl);

            if (!loadSiteMapResult.IsSuccess || loadSiteMapResult.HttpStatusCode != HttpStatusCode.OK)
                return new List<ContentLoadResult>();
            
            try
            {
                var siteMap = SiteMapSerializer.Deserialize(loadSiteMapResult.Content);
                ConsoleController.WriteLine.Success("sitemap.xml explored!");
                
                if (_settings.PrintResult) 
                    PrintToConsole(siteMap);

                return await ToContentLoadResultsAsync(siteMap, _exploreSettings);
            }
            catch (Exception e)
            {
                ConsoleController.WriteLine.Error($"Exploring error: {e.Message}");
                throw;
            }
        }

        private static void PrintToConsole(SiteMap siteMap)
        {
            if (siteMap.UrlSet == null)
                return;

            ConsoleController.WriteLine.Success("sitemap.xml exploring result: ");
            foreach (var url in siteMap.UrlSet)
            {
                ConsoleController.WriteLine.Info($"loc: {url.Loc}\tlastmod: {url.LastMod}\tchangefreq: " +
                                  $"{url.Changefreq}\tpriority: {url.Priority}");
            }
        }

        private async Task<IList<ContentLoadResult>> ToContentLoadResultsAsync(SiteMap siteMap, ExploreSettings exploreSettings)
        {
            var results = new List<ContentLoadResult>();

            if (siteMap.UrlSet == null) 
                return results;
            
            foreach (var url in siteMap.UrlSet)
            {
                if (url.Loc == null)
                    continue;

                if (exploreSettings.LoadContent && !exploreSettings.UrlsForExcludeLoadContent.Contains(url.Loc))
                {
                    results.Add(await _httpContentLoaderService.LoadContentAsync(url.Loc));
                    continue;
                }
                    
                results.Add(new ContentLoadResult(url.Loc));
            }

            return results;
        }
    }
}