using System.Threading.Tasks;

namespace SiteEvaluator.ConsoleUI
{
    public interface IConsoleView
    {
        Task<int> ScanHostAsync(string hostUrl, bool logToConsole = false, bool includeNofollowLinks = false);
        Task ShowUniqLinksInSiteMapAsync(string hostUrl);
        Task ShowUniqLinksByCrawlingAsync(string hostUrl);
        Task ShowCompositeReportAsync(string hostUrl);
    }
}