using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.Xml;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class SiteMapExplorerTests
    {
        [Theory]
        [ClassData(typeof(SitemapData))]
        public async Task ExploreAsync_HostUrlString_ShouldReturnSiteMapExploreResult(string sitemapXmlString, bool urlSetIsNull)
        {
            const string hostUrl = "https://localhost";
            
            var mockHttpContentLoader = new Mock<IHttpContentLoaderService>();
            mockHttpContentLoader
                .Setup(loader => loader.LoadSiteMapAsync(hostUrl))
                .Returns(Task.FromResult(GetContentLoadResult(hostUrl, sitemapXmlString)));

            var siteMapParseService = new SiteMapParseService();

            var siteMapExplorer = new SiteMapExplorer(mockHttpContentLoader.Object, siteMapParseService);

            var siteMapExploreResult = await siteMapExplorer.ExploreAsync(hostUrl);
            
            Assert.NotNull(siteMapExploreResult);
            Assert.Equal(urlSetIsNull, siteMapExploreResult.Count == 0);
            if (siteMapExploreResult.Count != 0)
            {
                Assert.Equal(2, siteMapExploreResult.Count);
                Assert.Contains( new ContentLoadResult(SitemapData.Url1!.Loc!), siteMapExploreResult);
                Assert.Contains(new ContentLoadResult(SitemapData.Url2!.Loc!), siteMapExploreResult);
            }
        }

        private ContentLoadResult GetContentLoadResult(string hostUrl, string sitemapXmlString)
        {
            var contentLoadResult = new ContentLoadResult(hostUrl);

            contentLoadResult.ApplyHttpResponseAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(sitemapXmlString)
            });

            return contentLoadResult;
        }
    }
}