using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MockHttp;
using SiteEvaluator.DataLoader.HttpLoader;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class HttpLoaderServiceTests
    {
        [Theory]
        [InlineData("http://i.ua", "http://i.ua", "Test content", HttpStatusCode.OK)]
        [InlineData("https://localhost", "https://localhost", "Test content", HttpStatusCode.OK)]
        [InlineData("http://i.ua", "http://aaa.ua", "", HttpStatusCode.NotFound)]
        public async Task LoadContentAsync_Url_ShouldReturnHttpExtendedResponse(
            string targetUrl, 
            string requestUrl, 
            string content, 
            HttpStatusCode statusCode)
        {
            var mockHttpMessageHandler = new MockHttpMessageHandler();

            mockHttpMessageHandler
                .Setup(targetUrl)
                .Returns(content);
            
            var httpContentLoader = new HttpLoaderService(new HttpClient(mockHttpMessageHandler));

            var httpExtendedResponse = await httpContentLoader.LoadAsync(new Uri(requestUrl));
            
            Assert.Equal(statusCode, httpExtendedResponse.StatusCode);
            Assert.Equal(content, httpExtendedResponse.Content.ReadAsStringAsync().Result);
        }
        
        [Fact]
        public async Task LoadContentAsync_WrongUrl_ShouldThrowUriFormatException()
        {
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.Setup("aaa.aa");

            var httpContentLoader = new HttpLoaderService(new HttpClient(mockHttpMessageHandler));

            await Assert.ThrowsAsync<UriFormatException>(() => httpContentLoader.LoadAsync(new Uri("aaa.aa")));
        }
        
        [Fact]
        public async Task LoadContentAsync_WrongUrl_ShouldThrowArgumentException()
        {
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.Setup("htt://aaa.aa");

            var httpContentLoader = new HttpLoaderService(new HttpClient(mockHttpMessageHandler));

            await Assert.ThrowsAsync<ArgumentException>(() => httpContentLoader.LoadAsync(new Uri("htt://aaa.aa")));
        }

        #region TestData
        
        public static IEnumerable<object[]> Data => new List<object[]>
        {
            new object[] {GetRobotsTxtWithSitemapUrlInLastLine(), HttpStatusCode.OK,  HttpStatusCode.OK, "Sitemap test content"},
            new object[] {GetRobotsTxtWithSitemapUrlInFirsLine(), HttpStatusCode.OK, HttpStatusCode.OK, "Sitemap test content"},
            //Sitemap URL is in robots.txt but page sitemap.xml actually missing
            new object[] {GetRobotsTxtWithSitemapUrlInFirsLine(), HttpStatusCode.NotFound, HttpStatusCode.NotFound, ""},
            new object[] {GetRobotsTxtWithoutSitemapUrl(), HttpStatusCode.NotFound, HttpStatusCode.NotFound, ""},
        };

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