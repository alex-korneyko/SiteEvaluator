using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.DataLoader;
using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExploring
{
    public class SiteMapExplorer : ISiteMapExplorer
    {
        private readonly IContentLoaderService _contentLoaderService;
        private readonly ISiteMapParseService _siteMapParseService;
        private readonly ExploreSettings _exploreSettings = new();

        public SiteMapExplorer(
            IContentLoaderService contentLoaderService,
            ISiteMapParseService siteMapParseService)
        {
            _contentLoaderService = contentLoaderService;
            _siteMapParseService = siteMapParseService;
        }

        public async Task<IList<PageInfo>> ExploreAsync(string hostUrl, Action<ExploreSettings>? exploreSettings = null)
        {
            exploreSettings?.Invoke(_exploreSettings);

            var loadSiteMapResult = await _contentLoaderService.LoadSiteMapAsync(hostUrl);

            if (!loadSiteMapResult.IsSuccess
                || loadSiteMapResult.HttpStatusCode != HttpStatusCode.OK
                || loadSiteMapResult.Content == null)
            {
                return new List<PageInfo>();
            }

            try
            {
                var siteMap = _siteMapParseService.DeserializeToSiteMap(loadSiteMapResult.Content);

                return await ToPageInfoListAsync(siteMap, _exploreSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exploring error: {e.Message}");
                throw;
            }
        }

        private async Task<IList<PageInfo>> ToPageInfoListAsync(SiteMap siteMap, ExploreSettings exploreSettings)
        {
            var results = new List<PageInfo>();

            if (siteMap.UrlSet == null) 
                return results;
            
            foreach (var url in siteMap.UrlSet)
            {
                if (url.Loc == null)
                    continue;

                if (exploreSettings.LoadContent && !exploreSettings.UrlsForExcludeLoadContent.Contains(url.Loc))
                {
                    var htmlLoadResult = await _contentLoaderService.LoadHtmlAsync(url.Loc);
                    var pageInfo = new PageInfo(htmlLoadResult);

                    results.Add(pageInfo);
                    _exploreSettings.ExploreHtmlLoadedEvent?.Invoke(htmlLoadResult);
                    
                    continue;
                }
                    
                results.Add(new PageInfo(new StringLoadResult(url.Loc)));
            }

            return results;
        }
    }
}