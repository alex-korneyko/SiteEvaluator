using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader.HttpLoader
{
    public class HttpLoaderService : IHttpLoaderService
    {
        private readonly HttpClient _httpClient;

        public HttpLoaderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpExtendedResponse> LoadAsync(Uri requestUri)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var httpResponseMessage = await _httpClient.GetAsync(requestUri);
            stopwatch.Stop();
            
            return new HttpExtendedResponse(httpResponseMessage, stopwatch.ElapsedMilliseconds);
        }
    }
}