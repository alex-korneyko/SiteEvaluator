using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SiteEvaluator.ContentLoader;
using SiteEvaluator.Data;
using SiteEvaluator.Presentation;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class ReportServiceTests
    {
        [Theory]
        [ClassData(typeof(ReportData))]
        public async Task AllMethodsInOneTest(
            IList<ContentLoadResult> crawlerResults,
            IList<ContentLoadResult> siteMapResults,
            IList<ContentLoadResult> expectedOnlyInCrawlerButNotInSiteMap,
            IList<ContentLoadResult> expectedOnlyInSiteMapButNotInCrawler,
            IList<ContentLoadResult> expectedCompositeResult)
        {
            var mockDao = new Mock<IDao<ContentLoadResult>>();
            
            mockDao.Setup(dao => dao.GetCrawlerResultsData("https://site.com"))
                .ReturnsAsync(crawlerResults);
            mockDao.Setup(dao => dao.GetSiteMapResultsData("https://site.com"))
                .ReturnsAsync(siteMapResults);

            IReportService reportService = new ReportService(mockDao.Object);

            var onlyInCrawlerResults = 
                (await reportService.GetUniqCrawlerResults("https://site.com"))
                .ToList();
            
            var onlyInSiteMapResults = 
                (await reportService.GetUniqInSiteMapResults("https://site.com"))
                .ToList();
            
            var compositeReport = 
                (await reportService.GetCompositeReportAsync("https://site.com"))
                .ToList();
            
            Assert.Equal(expectedOnlyInCrawlerButNotInSiteMap, onlyInCrawlerResults);
            Assert.Equal(expectedOnlyInSiteMapButNotInCrawler, onlyInSiteMapResults);
            Assert.Equal(expectedCompositeResult, compositeReport);
            mockDao.Verify(dao => dao.GetCrawlerResultsData("https://site.com"), Times.Exactly(3));
        }
        
        [Theory]
        [ClassData(typeof(ReportData))]
        public async Task AddCrawlerResultsAsync_HostUrl_ExpectedContentLoadResultsCollection(
            IList<ContentLoadResult> crawlerResults,
            IList<ContentLoadResult> siteMapResults,
            IList<ContentLoadResult> expectedOnlyInCrawlerButNotInSiteMap,
            IList<ContentLoadResult> expectedOnlyInSiteMapButNotInCrawler,
            IList<ContentLoadResult> compositeResult)
        {
            var mockDao = new Mock<IDao<ContentLoadResult>>();
            IReportService reportService = new ReportService(mockDao.Object);
            mockDao.Setup(dao => dao.SaveCrawlerResultsDataAsync("https://site.com", crawlerResults))
                .ReturnsAsync(crawlerResults.Count);
            
            await reportService.AddCrawlerResultsAsync("https://site.com", crawlerResults);
            
            mockDao.Verify(dao => dao.SaveCrawlerResultsDataAsync("https://site.com", crawlerResults), () => Times.Exactly(1));
        }
        
        [Theory]
        [ClassData(typeof(ReportData))]
        public async Task AddSiteMapExplorerResultsAsync_HostUrl_ExpectedContentLoadResultsCollection(
            IList<ContentLoadResult> crawlerResults,
            IList<ContentLoadResult> siteMapResults,
            IList<ContentLoadResult> expectedOnlyInCrawlerButNotInSiteMap,
            IList<ContentLoadResult> expectedOnlyInSiteMapButNotInCrawler,
            IList<ContentLoadResult> compositeResult)
        {
            var mockDao = new Mock<IDao<ContentLoadResult>>();
            IReportService reportService = new ReportService(mockDao.Object);
            mockDao.Setup(dao => dao.SaveCrawlerResultsDataAsync("https://site.com", siteMapResults))
                .ReturnsAsync(siteMapResults.Count);
            
            await reportService.AddCrawlerResultsAsync("https://site.com", siteMapResults);
            
            mockDao.Verify(dao => dao.SaveCrawlerResultsDataAsync("https://site.com", siteMapResults), () => Times.Exactly(1));
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