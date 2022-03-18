using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.Common;
using SiteEvaluator.DataLoader;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Nodes;
using SiteEvaluator.SiteMapExploring.Parser;

namespace SiteEvaluator.SiteMapExploring
{
    public class SiteMapExplorer : ISiteMapExplorer
    {
        private readonly IContentLoaderService _contentLoaderService;
        private readonly ISiteMapParseService _siteMapParseService;
        private readonly IHtmlParseService _htmlParseService;
        private readonly ExploreSettings _exploreSettings = new();

        public SiteMapExplorer(
            IContentLoaderService contentLoaderService,
            ISiteMapParseService siteMapParseService,
            IHtmlParseService htmlParseService)
        {
            _contentLoaderService = contentLoaderService;
            _siteMapParseService = siteMapParseService;
            _htmlParseService = htmlParseService;
        }

        public async Task<IList<PageInfo>> ExploreAsync(string hostUrl, Action<ExploreSettings>? exploreSettings = null)
        {
            exploreSettings?.Invoke(_exploreSettings);

            var hostUri = new Uri(hostUrl);
            var loadSiteMapResult = await _contentLoaderService.LoadSiteMapAsync(hostUri);

            if (!loadSiteMapResult.IsSuccess
                || loadSiteMapResult.HttpStatusCode != HttpStatusCode.OK
                || loadSiteMapResult.Content == null)
            {
                return new List<PageInfo>();
            }

            try
            {
                var siteMap = _siteMapParseService.DeserializeToSiteMap(loadSiteMapResult.Content);

                return await ToPageInfoListAsync(siteMap, hostUri, _exploreSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exploring error: {e.Message}");
                throw;
            }
        }

        private async Task<IList<PageInfo>> ToPageInfoListAsync(SiteMap siteMap, Uri hostUri, ExploreSettings exploreSettings)
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
                    var htmlLoadResult = await _contentLoaderService.LoadHtmlAsync(new Uri(url.Loc, UriKind.Absolute));
                    var pageInfo = new PageInfo(htmlLoadResult);

                    results.Add(pageInfo);
                    _exploreSettings.ExploreHtmlLoadedEvent?.Invoke(htmlLoadResult);

                    var allANodes = _htmlParseService.GetAllNodes<A>(pageInfo.Content);

                    pageInfo.OuterUrls = Utils.FilterOuterLinksNodes(allANodes, hostUri)
                        .Select(aNode => aNode.Href)
                        .ToList()!;
                    
                    pageInfo.InnerUrls = Utils.FilterInnerLinkNodes(allANodes, hostUri)
                        .Select(aNode => aNode.Href)
                        .ToList()!;

                    var allImgNodes = _htmlParseService.GetAllNodes<Img>(pageInfo.Content);
                    await _contentLoaderService.ScanAndApplyMediaLinks(
                        pageInfo,
                        allImgNodes,
                        _exploreSettings.LoadMedia,
                        _exploreSettings.ExploreImageLoadedEvent);
                    
                    continue;
                }

                results.Add(new PageInfo(new StringLoadResult(url.Loc)));
            }

            return results;
        }
    }
}