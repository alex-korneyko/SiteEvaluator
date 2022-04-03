using System;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.ConsoleUI
{
    public interface IConsoleView
    {
        Task<TargetHost> ScanHostAsync(Uri hostUri, bool logToConsole = false, bool includeNofollowLinks = false);
        Task ShowUniqLinksInSiteMapAsync(Uri hostUri);
        Task ShowUniqLinksByCrawlingAsync(Uri hostUri);
        Task ShowCompositeReportAsync(Uri hostUri);
    }
}