using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SiteEvaluator.Common;
using SiteEvaluator.Data.DataHandlers;
using SiteEvaluator.Data.Model;
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
        private readonly IDataHandlerService _dataHandlerService;
        private readonly ExploreSettings _exploreSettings = new();

        public SiteMapExplorer(
            IContentLoaderService contentLoaderService,
            ISiteMapParseService siteMapParseService,
            IHtmlParseService htmlParseService,
            IDataHandlerService dataHandlerService)
        {
            _contentLoaderService = contentLoaderService;
            _siteMapParseService = siteMapParseService;
            _htmlParseService = htmlParseService;
            _dataHandlerService = dataHandlerService;
        }

        public async Task<TargetHost> ExploreAsync(Uri hostUri, Action<ExploreSettings>? exploreSettings = null)
        {
            exploreSettings?.Invoke(_exploreSettings);

            var loadSiteMapResult = await _contentLoaderService.LoadSiteMapAsync(hostUri);

            if (!loadSiteMapResult.IsSuccess ||
                loadSiteMapResult.HttpStatusCode != HttpStatusCode.OK ||
                loadSiteMapResult.Content == null)
            {
                var targetHost = new TargetHost(hostUri)
                {
                    SiteMapError = true
                };
                
                return await _dataHandlerService.SaveTargetHostAsync(targetHost);
            }

            try
            {
                var siteMap = _siteMapParseService.DeserializeToSiteMap(loadSiteMapResult.Content);

                var targetHost = await ToPageInfoListAsync(siteMap, hostUri, _exploreSettings);

                return await _dataHandlerService.SaveTargetHostAsync(targetHost);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exploring error: {e.Message}");
                throw;
            }
        }

        private async Task<TargetHost> ToPageInfoListAsync(SiteMap siteMap, Uri hostUri, ExploreSettings exploreSettings)
        {
            var targetHost = new TargetHost(hostUri);

            if (siteMap.UrlSet == null)
            {
                targetHost.SiteMapError = true;
                
                return targetHost;
            }

            foreach (var url in siteMap.UrlSet)
            {
                if (url.Loc == null)
                    continue;

                if (exploreSettings.LoadContent && !exploreSettings.UrlsForExcludeLoadContent.Contains(url.Loc))
                {
                    var htmlLoadResult = await _contentLoaderService.LoadHtmlAsync(new Uri(url.Loc, UriKind.Absolute));
                    var pageInfo = new PageInfo(htmlLoadResult, ScannerType.SiteMap);

                    targetHost.PageInfos.Add(pageInfo);
                    _exploreSettings.ExploreHtmlLoadedEvent?.Invoke(htmlLoadResult);

                    var allANodes = _htmlParseService.GetAllNodes<A>(pageInfo.Content);

                    pageInfo.PageInfoUrls.AddRange(Utils.FilterOuterLinksNodes(allANodes, hostUri)
                        .Select(aNode => new PageInfoUrl(aNode.Href, PageInfoUrlType.Outer))
                        .ToList());
                    
                    pageInfo.PageInfoUrls.AddRange(Utils.FilterInnerLinkNodes(allANodes, hostUri)
                        .Select(aNode => new PageInfoUrl(aNode.Href, PageInfoUrlType.Inner))
                        .ToList());

                    var allImgNodes = _htmlParseService.GetAllNodes<Img>(pageInfo.Content);
                    await _contentLoaderService.ScanAndApplyMediaLinks(
                        pageInfo,
                        allImgNodes,
                        _exploreSettings.LoadMedia,
                        _exploreSettings.ExploreImageLoadedEvent);
                    
                    continue;
                }

                targetHost.PageInfos.Add(new PageInfo(new StringLoadResult(url.Loc), ScannerType.SiteMap));
            }

            return targetHost;
        }
    }
}