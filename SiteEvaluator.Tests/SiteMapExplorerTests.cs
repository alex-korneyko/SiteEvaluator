using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using SiteEvaluator.Data.Model;
using SiteEvaluator.DataLoader;
using SiteEvaluator.DataLoader.HttpLoader;
using SiteEvaluator.Html;
using SiteEvaluator.SiteMapExploring;
using SiteEvaluator.SiteMapExploring.Parser;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class SiteMapExplorerTests
    {
        [Theory]
        [ClassData(typeof(SitemapData))]
        public async Task ExploreAsync_HostUrlString_ShouldReturnPageInfos(string sitemapXmlString, bool urlSetIsNull)
        {
            const string hostUrl = "https://localhost";
            
            var mockHttpContentLoader = new Mock<IContentLoaderService>();
            mockHttpContentLoader
                .Setup(loader => loader.LoadSiteMapAsync(new Uri(hostUrl)))
                .Returns(Task.FromResult(GetContentLoadResult(hostUrl, sitemapXmlString)));

            var siteMapParseService = new SiteMapParseService();
            var htmlParseService = new HtmlParseService();

            var siteMapExplorer = new SiteMapExplorer(mockHttpContentLoader.Object, siteMapParseService, htmlParseService);

            var pageInfos = await siteMapExplorer.ExploreAsync(hostUrl);
            
            Assert.NotNull(pageInfos);
            Assert.Equal(urlSetIsNull, pageInfos.Count == 0);
            if (pageInfos.Count != 0)
            {
                Assert.Equal(2, pageInfos.Count);
                var pageInfoUrl1 = new PageInfo(new StringLoadResult(SitemapData.Url1!.Loc!), "https://localhost", ScannerType.SiteMap);
                var pageInfoUrl2 = new PageInfo(new StringLoadResult(SitemapData.Url2!.Loc!), "https://localhost", ScannerType.SiteMap);
                Assert.Contains( pageInfoUrl1, pageInfos);
                Assert.Contains(pageInfoUrl2, pageInfos);
            }
        }

        private StringLoadResult GetContentLoadResult(string hostUrl, string sitemapXmlString)
        {
            var stringLoadResult = new StringLoadResult(hostUrl);

            stringLoadResult.ApplyHttpResponseAsync(new HttpExtendedResponse(new HttpResponseMessage(HttpStatusCode.OK), 100)
            {
                Content = new StringContent(sitemapXmlString)
            });

            return stringLoadResult;
        }
    }
}