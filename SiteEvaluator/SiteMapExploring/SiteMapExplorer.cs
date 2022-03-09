using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Presentation;
using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExploring
{
    public class SiteMapExplorer : ISiteMapExplorer
    {
        private readonly IHttpContentLoader _httpContentLoader;
        private readonly SiteMapExplorerSettings _settings = new();

        public SiteMapExplorer(IHttpContentLoader httpContentLoader)
        {
            _httpContentLoader = httpContentLoader;
        }

        public SiteMapExplorer(IHttpContentLoader httpContentLoader, Action<SiteMapExplorerSettings> explorerSettings) : this(
            httpContentLoader)
        {
            explorerSettings.Invoke(_settings);
        }

        public async Task<IList<ContentLoadResult>> ExploreAsync(string hostUrl, bool loadPagesContent)
        {
            ConsoleController.WriteLine.Warning("Start sitemap.xml exploring...");

            var loadSiteMapResult = await _httpContentLoader.LoadSiteMapAsync(hostUrl);

            if (!loadSiteMapResult.IsSuccess || loadSiteMapResult.HttpStatusCode != HttpStatusCode.OK)
                return new List<ContentLoadResult>();
            
            try
            {
                var siteMap = SiteMapSerializer.Deserialize(loadSiteMapResult.Content);
                ConsoleController.WriteLine.Success("sitemap.xml explored!");
                
                if (_settings.PrintResult) 
                    PrintToConsole(siteMap);

                return await ToContentLoadResultsAsync(siteMap, loadPagesContent);
            }
            catch (Exception e)
            {
                ConsoleController.WriteLine.Error($"Exploring error: {e.Message}");
                throw;
            }
        }

        private static void PrintToConsole(SiteMap siteMap)
        {
            if (siteMap.UrlSet == null) return;

            ConsoleController.WriteLine.Success("sitemap.xml exploring result: ");
            foreach (var url in siteMap.UrlSet)
            {
                ConsoleController.WriteLine.Info($"loc: {url.Loc}\tlastmod: {url.LastMod}\tchangefreq: " +
                                  $"{url.Changefreq}\tpriority: {url.Priority}");
            }
        }

        private async Task<IList<ContentLoadResult>> ToContentLoadResultsAsync(SiteMap siteMap, bool loadPagesContent)
        {
            return loadPagesContent 
                ? await GetFullContentLoadResultsAsync(siteMap)
                : GetEmptyContentLoadResults(siteMap);
        }

        private IList<ContentLoadResult> GetEmptyContentLoadResults(SiteMap siteMap)
        {
            if (siteMap.UrlSet != null)
            {
                return siteMap.UrlSet
                    .Where(url => url.Loc != null)
                    .Select(url => new ContentLoadResult(url.Loc!))
                    .ToList();
            }

            return new List<ContentLoadResult>();
        }

        private async Task<List<ContentLoadResult>> GetFullContentLoadResultsAsync(SiteMap siteMap)
        {
            var fullContentLoadResults = new List<ContentLoadResult>();

            if (siteMap.UrlSet != null)
            {
                foreach (var url in siteMap.UrlSet)
                {
                    fullContentLoadResults.Add(await _httpContentLoader.LoadContentAsync(url.Loc!));
                }
            }

            return fullContentLoadResults;
        }
    }
}