using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader
{
    public class StringLoadResult : ContentLoadResult<string>
    {
        public StringLoadResult(string requestedUrl) : base(requestedUrl)
        {
        }

        protected override async Task<string?> ApplyContent(HttpContent httpContent)
        {
            var stringContent = await httpContent.ReadAsStringAsync();
            
            return stringContent;
        }
    }
}