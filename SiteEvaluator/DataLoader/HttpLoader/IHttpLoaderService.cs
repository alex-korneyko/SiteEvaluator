using System;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader.HttpLoader
{
    public interface IHttpLoaderService
    {
        Task<HttpExtendedResponse> LoadAsync(Uri requestUri);
    }
}