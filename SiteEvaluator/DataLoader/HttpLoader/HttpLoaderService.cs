using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader.HttpLoader
{
    public class HttpLoaderService : IHttpLoaderService
    {
        private HttpClient _httpClient;

        public HttpLoaderService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpExtendedResponse> LoadAsync(Uri requestUri)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var httpResponseMessage = await _httpClient.GetAsync(requestUri);
            stopwatch.Stop();
            
            return new HttpExtendedResponse(httpResponseMessage, stopwatch.ElapsedMilliseconds);
        }

        public void SetHttpMessageHandler(HttpMessageHandler httpMessageHandler)
        {
            _httpClient = new HttpClient(httpMessageHandler);
        }
    }
}