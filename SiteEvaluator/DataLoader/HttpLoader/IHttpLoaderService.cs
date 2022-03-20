using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader.HttpLoader
{
    public interface IHttpLoaderService
    {
        Task<HttpExtendedResponse> LoadAsync(Uri requestUri);
        void SetHttpMessageHandler(HttpMessageHandler httpMessageHandler);
    }
}