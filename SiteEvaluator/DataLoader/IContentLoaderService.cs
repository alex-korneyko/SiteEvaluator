using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteEvaluator.Html.Nodes;

namespace SiteEvaluator.DataLoader
{
    public interface IContentLoaderService
    {
        Task<StringLoadResult> LoadRobotsAsync(Uri requestUri);
        Task<StringLoadResult> LoadSiteMapAsync(Uri requestUri);
        Task<StringLoadResult> LoadHtmlAsync(Uri requestUri);
        Task<ImageLoadResult> LoadImageAsync(Uri requestUri);
        Task<FileLoadResult> LoadFile(Uri requestUri);
        Task ScanAndApplyMediaLinks(
            PageInfo pageInfo,
            IList<Img> allImgNodes,
            bool loadImgContent = false,
            Action<ImageLoadResult>? imageLoadedEvent = null);
    }
}