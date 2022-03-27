using System;
using System.Collections;
using System.Collections.Generic;
using SiteEvaluator.SiteMapExploring.Parser;

namespace SiteEvaluator.Tests
{
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
        
        public static Url Url1 => new()
        {
            Loc = "http://www.site.com/",
            LastMod = DateTime.Parse("2022-01-19T16:03:22+01:00"),
            ChangeFreq = "monthly",
            Priority = 1.0f
        };
        public static Url Url2 => new()
        {
            Loc = "http://www.site.com/page-1.html",
            LastMod = DateTime.Parse("2022-01-19"),
            ChangeFreq = "monthly",
            Priority = 0.8f
        };
    }
}