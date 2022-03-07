using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using SiteEvaluator.ContentLoader;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class HttpContentLoaderTests
    {
        [Theory]
        [InlineData("http://i.ua", "http://i.ua", "Test content", HttpStatusCode.OK)]
        [InlineData("https://localhost", "https://localhost", "Test content", HttpStatusCode.OK)]
        [InlineData("http://i.ua", "http://aaa.ua", "", HttpStatusCode.NotFound)]
        public async Task LoadContentAsync_Url_ShouldReturnContentLoadResult(
            string targetUrl, 
            string requestUrl, 
            string content, 
            HttpStatusCode statusCode)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(targetUrl)
                .Respond("text/html", content);

            var httpContentLoader = new HttpContentLoader(new HttpClient(mockHttp));

            var contentLoadResult = await httpContentLoader.LoadContentAsync(requestUrl);
            
            Assert.True(contentLoadResult.IsSuccess);
            Assert.Equal(statusCode, contentLoadResult.HttpStatusCode);
            Assert.Equal(content, contentLoadResult.Content);
        }
        
        [Theory]
        [InlineData("htt://aaa.aa")]
        [InlineData("aaa.aa")]
        public async Task LoadContentAsync_WrongUrl_ShouldReturnContentLoadResultWithException(string url)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(url);

            var httpContentLoader = new HttpContentLoader(new HttpClient(mockHttp));

            var contentLoadResult = await httpContentLoader.LoadContentAsync(url);

            Assert.False(contentLoadResult.IsSuccess);
            Assert.IsType<ArgumentException>(contentLoadResult.Exception);
        }
        
        [Fact]
        public async Task LoadRobotsAsync_UrlString_ShouldReturnContentLoadResult()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When("https://localhost/robots.txt")
                .Respond("text/html", GetRobotsTxtWithSitemapUrlInLastLine());

            var httpClient = mockHttp.ToHttpClient();

            var httpContentLoader = new HttpContentLoader(httpClient);
            var contentLoadResult = await httpContentLoader.LoadRobotsAsync("https://localhost");
            
            Assert.True(contentLoadResult.IsSuccess);
            Assert.Equal(GetRobotsTxtWithSitemapUrlInLastLine(), contentLoadResult.Content);
        }
        
        [Theory]
        [MemberData(nameof(Data))]
        public async Task LoadSiteMapAsync_UrlString_ShouldReturnContentLoadResult(
            string robotsContent, 
            HttpStatusCode expectedStatusCode,
            HttpStatusCode actualStatusCode,
            string sitemapContent)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When("https://localhost/robots.txt")
                .Respond("text/html", robotsContent);

            mockHttp
                .When("https://localhost/serviceInformation/sitemap.xml")
                .Respond(actualStatusCode, "text/html", sitemapContent);
            
            var httpClient = mockHttp.ToHttpClient();

            var httpContentLoader = new HttpContentLoader(httpClient);
            var contentLoadResult = await httpContentLoader.LoadSiteMapAsync("https://localhost");
            
            Assert.True(contentLoadResult.IsSuccess);
            Assert.Equal(expectedStatusCode, contentLoadResult.HttpStatusCode);
            Assert.Equal(sitemapContent, contentLoadResult.Content);
        }
        
        public static IEnumerable<object[]> Data => new List<object[]>
        {
            new object[] {GetRobotsTxtWithSitemapUrlInLastLine(), HttpStatusCode.OK,  HttpStatusCode.OK, "Sitemap test content"},
            new object[] {GetRobotsTxtWithSitemapUrlInFirsLine(), HttpStatusCode.OK, HttpStatusCode.OK, "Sitemap test content"},
            //Sitemap URL present in robots.txt but page sitemap.xml actually missing
            new object[] {GetRobotsTxtWithSitemapUrlInFirsLine(), HttpStatusCode.NotFound, HttpStatusCode.NotFound, ""},
            new object[] {GetRobotsTxtWithoutSitemapUrl(), HttpStatusCode.NotFound, HttpStatusCode.NotFound, ""},
        };

        #region TestData

        private static string GetRobotsTxtWithoutSitemapUrl()
        {
            return "\nUser-agent: *" +
                   "\nAllow: /" +
                   "\nDisallow: /cookie-policy" +
                   "\nDisallow: /contacts";
        }
        
        private static string GetRobotsTxtWithSitemapUrlInLastLine()
        {
            return  GetRobotsTxtWithoutSitemapUrl() + "\nSitemap: https://localhost/serviceInformation/sitemap.xml";
        }
        
        private static string GetRobotsTxtWithSitemapUrlInFirsLine()
        {
            return  "\nSitemap: https://localhost/serviceInformation/sitemap.xml" + GetRobotsTxtWithoutSitemapUrl();
        }
        
        #endregion
    }
}