using System.Net.Http;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.DataLoader
{
    public class StringLoadResult :
        ContentLoadResult<string>,
        IPageInfoLoadResult
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