using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;

namespace SiteEvaluator.Presentation
{
    public class Report
    {
        private readonly StringBuilder _reportString = new();
        private readonly List<ContentLoadResult> _crawlerResults = new();
        private readonly List<ContentLoadResult> _siteMapResults = new();

        public void AddCrawlerResults(IList<ContentLoadResult> crawlerResults)
        {
            _crawlerResults.AddRange(crawlerResults);
            _reportString.Append($"Urls(html documents) found after crawling a website: {crawlerResults.Count}");
            
            ConsoleController.WriteLine.Note($"Total crawled pages: {crawlerResults.Count}\n");
        }

        public void AddSiteMapExplorerResults(IList<ContentLoadResult> siteMapExploreResults)
        {
            _reportString.Append($"\nUrls found in sitemap: {siteMapExploreResults.Count}");
            _siteMapResults.AddRange(siteMapExploreResults);
            
            ConsoleController.WriteLine.Note($"Total in sitemap.xml: {siteMapExploreResults.Count}\n");
        }
        
        public async Task ReportDifferences()
        {
            var onlyInSiteMapResults = SubtractLists(_siteMapResults, _crawlerResults).ToList();
            var onlyInCrawlerResults = SubtractLists(_crawlerResults, _siteMapResults).ToList();

            ReportExclusive(onlyInSiteMapResults, "Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");
            ReportExclusive(onlyInCrawlerResults, "Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
            
            ConsoleController.WriteLine.Success("\nList of pages sorted by loading time (ms):");
            
            var onlyInSiteMapResultsWithContent = await LoadPagesContentAsync(onlyInSiteMapResults);
            var contentLoadResults = JoinSort(_crawlerResults, onlyInSiteMapResultsWithContent);
            
            foreach (var contentLoadResult in contentLoadResults)
            {
                Console.WriteLine(contentLoadResult);
            }
        }

        public void ReportSummary()
        {
            ConsoleController.WriteLine.Success($"\n{_reportString}");
            ConsoleController.ReadLine.Warning("Press ENTER for exit");
        }
        
        private static async Task<IList<ContentLoadResult>> LoadPagesContentAsync(IEnumerable<ContentLoadResult> emptyResults)
        {
            var exclusivelyForSiteMap = new List<ContentLoadResult>();
            
            var httpContentLoader = new HttpContentLoader(new HttpClient());
            foreach (var contentLoadResult in emptyResults)
            {
                exclusivelyForSiteMap.Add(await httpContentLoader.LoadContentAsync(contentLoadResult.PageUrl));
            }

            return exclusivelyForSiteMap;
        }

        private static void ReportExclusive(List<ContentLoadResult> contentLoadResults, string header)
        {
            ConsoleController.WriteLine.Success(header);
            contentLoadResults.ForEach(item => Console.WriteLine(item.PageUrl));
            ConsoleController.WriteLine.Note($"Total: {contentLoadResults.Count}\n");
        }
        
        private static IEnumerable<T> SubtractLists<T>(IEnumerable<T> list1, IEnumerable<T> list2)
            where T : IEquatable<T>
        {
            return list1.Where(item => !list2.Contains(item));
        }

        private static IEnumerable<T> JoinSort<T>(params IEnumerable<T>[] lists)
        {
            var result = new List<T>();
            foreach (var list in lists)
            {
                result.AddRange(list);
            }
            
            result.Sort();

            return result;
        }
    }
}