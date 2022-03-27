using SiteEvaluator.SiteMapExploring.Parser;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class SiteMapSerializerTests
    {
        [Theory]
        [ClassData(typeof(SitemapData))]
        public void Deserialize_XmlString_ShouldReturnSiteMapObject(string sitemapXmlString, bool urlSetIsNull)
        {
            var siteMapParseService = new SiteMapParseService();

            var siteMap = siteMapParseService.DeserializeToSiteMap(sitemapXmlString);
            
            Assert.NotNull(siteMap);
            Assert.IsType<SiteMap>(siteMap);
            Assert.Equal(urlSetIsNull, siteMap.UrlSet == null);
            if (siteMap.UrlSet != null)
            {
                Assert.NotEmpty(siteMap.UrlSet);
                Assert.Equal(2, siteMap.UrlSet.Length);
                Assert.Contains(SitemapData.Url1, siteMap.UrlSet);
                Assert.Contains(SitemapData.Url2, siteMap.UrlSet);
            }
        }
    }
}