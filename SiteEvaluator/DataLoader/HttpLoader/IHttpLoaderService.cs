using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader.HttpLoader
{
    public interface IHttpLoaderService
    {
        Task<HttpExtendedResponse> LoadAsync(string requestUri);
    }
}