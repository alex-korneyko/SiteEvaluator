using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader
{
    public class FileLoadResult : ContentLoadResult<byte[]>
    {
        public FileLoadResult(string requestedUrl) : base(requestedUrl)
        {
        }

        protected override async Task<byte[]?> ApplyContent(HttpContent httpContent)
        {
            return await httpContent.ReadAsByteArrayAsync();
        }
    }
}