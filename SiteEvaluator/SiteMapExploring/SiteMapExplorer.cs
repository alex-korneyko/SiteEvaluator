using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExploring
{
    public class SiteMapExplorer : ISiteMapExplorer
    {
        private readonly IHttpContentLoaderService _httpContentLoaderService;
        private readonly ISiteMapParseService _siteMapParseService;
        private readonly ExploreSettings _exploreSettings = new();

        public SiteMapExplorer(
            IHttpContentLoaderService httpContentLoaderService,
            ISiteMapParseService siteMapParseService)
        {
            _httpContentLoaderService = httpContentLoaderService;
            _siteMapParseService = siteMapParseService;
        }

        public async Task<IList<ContentLoadResult>> ExploreAsync(string hostUrl, Action<ExploreSettings>? exploreSettings = null)
        {
            exploreSettings?.Invoke(_exploreSettings);

            var loadSiteMapResult = await _httpContentLoaderService.LoadSiteMapAsync(hostUrl);

            if (!loadSiteMapResult.IsSuccess || loadSiteMapResult.HttpStatusCode != HttpStatusCode.OK)
                return new List<ContentLoadResult>();
            
            try
            {
                var siteMap = _siteMapParseService.DeserializeToSiteMap(loadSiteMapResult.Content);

                return await ToContentLoadResultsAsync(siteMap, _exploreSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exploring error: {e.Message}");
                throw;
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
                    var contentLoadResult = await _httpContentLoaderService.LoadContentAsync(url.Loc);
                    
                    results.Add(contentLoadResult);
                    _exploreSettings.ExploreEvent?.Invoke(contentLoadResult);
                    
                    continue;
                }
                    
                results.Add(new ContentLoadResult(url.Loc));
            }

            return results;
        }
    }
}