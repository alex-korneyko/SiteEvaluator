using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader
{
    public class StringLoadResult : ContentLoadResult<string>
    {
        public StringLoadResult(string pageUrl) : base(pageUrl)
        {
        }

        protected override async Task<string> ApplyContent(HttpContent httpContent)
        {
            var stringContent = await httpContent.ReadAsStringAsync();
            
            return stringContent;
        }
    }
}