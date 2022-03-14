using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Presentation;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class ReportServiceTests
    {
        [Theory]
        [ClassData(typeof(ReportData))]
        public void AllMethodsInOneTest(
            IEnumerable<ContentLoadResult> crawlerResults,
            IEnumerable<ContentLoadResult> siteMapResults,
            IEnumerable<ContentLoadResult> expectedOnlyInCrawlerButNotInSiteMap,
            IEnumerable<ContentLoadResult> expectedOnlyInSiteMapButNotInCrawler,
            IEnumerable<ContentLoadResult> compositeResult)
        {
            // IReportService reportService = new ReportService();
            // reportService.AddCrawlerResultsAsync(crawlerResults);
            // reportService.AddSiteMapExplorerResultsAsync(siteMapResults);
            //
            // var onlyInCrawlerResults = reportService.GetUniqCrawlerResults().ToList();
            // var onlyInSiteMapResults = reportService.GetUniqInSiteMapResults().ToList();
            //
            // var compositeReport = reportService.GetCompositeReportAsync().ToList();
            //
            // Assert.Empty(onlyInCrawlerResults.Except(expectedOnlyInCrawlerButNotInSiteMap));
            //
            // Assert.Empty(onlyInSiteMapResults.Except(expectedOnlyInSiteMapButNotInCrawler));
            //
            // Assert.Empty(compositeResult.Except(compositeReport));
        }
    }

    class ReportData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                CrawlerResults,
                SiteMapResults,
                OnlyInCrawlerButNotInSiteMap,
                OnlyInSiteMapButNotInCrawler,
                CompositeResult
            };

            yield return new object[]
            {
                CrawlerResults,
                new List<ContentLoadResult>(),
                CrawlerResults,
                new List<ContentLoadResult>(),
                CrawlerResults
            };
            
            yield return new object[]
            {
                new List<ContentLoadResult>(),
                CrawlerResults,
                new List<ContentLoadResult>(),
                CrawlerResults,
                CrawlerResults
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<ContentLoadResult> CrawlerResults =>
            new List<ContentLoadResult>
            {
                new("link1"),
                new("link2"),
                new("link3"),
                new("link4"),
                new("link6"),
                new("link7")
            };

        private IEnumerable<ContentLoadResult> OnlyInCrawlerButNotInSiteMap =>
            new List<ContentLoadResult>
            {
                new("link6"),
                new("link7")
            };

        private IEnumerable<ContentLoadResult> SiteMapResults =>
            new List<ContentLoadResult>
            {
                new("link1"),
                new("link2"),
                new("link3"),
                new("link4"),
                new("link8"),
                new("link9")
            };
        
        private IEnumerable<ContentLoadResult> OnlyInSiteMapButNotInCrawler =>
            new List<ContentLoadResult>
            {
                new("link8"),
                new("link9")
            };
        
        private IEnumerable<ContentLoadResult> CompositeResult => 
            new List<ContentLoadResult>
            {
                new("link1"),
                new("link2"),
                new("link3"),
                new("link4"),
                new("link6"),
                new("link7"),
                new("link8"),
                new("link9")
            };
    }
}