using System;
using System.Collections;
using System.Collections.Generic;
using SiteEvaluator.Xml;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class SiteMapSerializerTests
    {
        [Theory]
        [ClassData(typeof(SitemapData))]
        public void Deserialize_XmlString_ShouldReturnSiteMapObject(string sitemapXmlString, bool urlSetIsNull)
        {
            var siteMap = SiteMapSerializer.Deserialize(sitemapXmlString);
            
            Assert.NotNull(siteMap);
            Assert.IsType<SiteMap>(siteMap);
            Assert.Equal(urlSetIsNull, siteMap.UrlSet == null);
            if (siteMap.UrlSet != null)
            {
                Assert.NotEmpty(siteMap.UrlSet);
                Assert.Equal(2, siteMap.UrlSet.Length);
                Assert.Contains(Url1, siteMap.UrlSet);
                Assert.Contains(Url2, siteMap.UrlSet);
            }
        }

        #region VerificationData

        private static Url Url1 => new()
        {
            Loc = "http://www.site.com/",
            LastMod = DateTime.Parse("2022-01-19T16:03:22+01:00"),
            Changefreq = "monthly",
            Priority = 1.0f
        };
        private static Url Url2 => new()
        {
            Loc = "http://www.site.com/page-1.html",
            LastMod = DateTime.Parse("2022-01-19"),
            Changefreq = "monthly",
            Priority = 0.8f
        };

        #endregion
    }

    class SitemapData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {SiteMap, false};
            yield return new object[] {"", true};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        private string SiteMap => "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">" +
                                  "  <url>" +
                                  "    <loc>http://www.site.com/</loc>" +
                                  "    <lastmod>2022-01-19T16:03:22+01:00</lastmod>" +
                                  "    <changefreq>monthly</changefreq>" +
                                  "    <priority>1.0</priority>" +
                                  "  </url>" +
                                  "  <url>" +
                                  "    <loc>http://www.site.com/page-1.html</loc>" +
                                  "    <lastmod>2022-01-19</lastmod>" +
                                  "    <changefreq>monthly</changefreq>" +
                                  "    <priority>0.8</priority>" +
                                  "  </url>" +
                                  "</urlset>";
    }
}