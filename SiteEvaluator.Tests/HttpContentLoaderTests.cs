using System;
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
        
        
    }
}