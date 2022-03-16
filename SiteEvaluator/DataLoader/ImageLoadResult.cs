using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.DataLoader
{
    public class ImageLoadResult : ContentLoadResult<Bitmap>
    {
        public ImageLoadResult(string pageUrl) : base(pageUrl)
        {
        }

        protected override async Task<Bitmap?> ApplyContent(HttpContent httpContent)
        {
            var asStream = await httpContent.ReadAsStreamAsync();

            try
            {
                var bitmap = new Bitmap(asStream);

                return bitmap;
            }
            catch (Exception e)
            {
                // ignored
            }

            return null;
        }
    }
}