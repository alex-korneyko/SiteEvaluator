using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using SiteEvaluator.ContentLoader;
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
            
            var mockHttpContentLoader = new Mock<IHttpContentLoader>();
            mockHttpContentLoader
                .Setup(loader => loader.LoadSiteMapAsync(hostUrl))
                .Returns(Task.FromResult(GetContentLoadResult(hostUrl, sitemapXmlString)));
            

            var siteMapExplorer = new SiteMapExplorer.SiteMapExplorer(mockHttpContentLoader.Object);

            var siteMapExploreResult = await siteMapExplorer.ExploreAsync(hostUrl);
            
            Assert.NotNull(siteMapExploreResult);
            Assert.Equal(urlSetIsNull, siteMapExploreResult.SiteMap.UrlSet == null);
            if (siteMapExploreResult.SiteMap.UrlSet != null)
            {
                Assert.Equal(2, siteMapExploreResult.SiteMap.UrlSet.Length);
                Assert.Contains(SitemapData.Url1, siteMapExploreResult.SiteMap.UrlSet);
                Assert.Contains(SitemapData.Url2, siteMapExploreResult.SiteMap.UrlSet);
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