using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SiteEvaluator.Data;
using SiteEvaluator.DataLoader;
using SiteEvaluator.Presentation;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class ReportServiceTests
    {
        [Theory]
        [ClassData(typeof(ReportData))]
        public async Task AllMethodsInOneTest(
            IList<PageInfo> crawlerResults,
            IList<PageInfo> siteMapResults,
            IList<PageInfo> expectedOnlyInCrawlerButNotInSiteMap,
            IList<PageInfo> expectedOnlyInSiteMapButNotInCrawler,
            IList<PageInfo> expectedCompositeResult)
        {
            var mockDao = new Mock<IDao<PageInfo>>();
            
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
        public async Task AddCrawlerResultsAsync_HostUrl_ExpectedInvokeSaveCrawlerResultsDataAsync(
            IList<PageInfo> crawlerResults,
            IList<PageInfo> siteMapResults,
            IList<PageInfo> expectedOnlyInCrawlerButNotInSiteMap,
            IList<PageInfo> expectedOnlyInSiteMapButNotInCrawler,
            IList<PageInfo> compositeResult)
        {
            var mockDao = new Mock<IDao<PageInfo>>();
            IReportService reportService = new ReportService(mockDao.Object);
            mockDao.Setup(dao => dao.SaveCrawlerResultsDataAsync("https://site.com", crawlerResults))
                .ReturnsAsync(crawlerResults.Count);
            
            await reportService.AddCrawlerResultsAsync("https://site.com", crawlerResults);
            
            mockDao.Verify(dao => dao.SaveCrawlerResultsDataAsync("https://site.com", crawlerResults), () => Times.Exactly(1));
        }
        
        [Theory]
        [ClassData(typeof(ReportData))]
        public async Task AddSiteMapExplorerResultsAsync_HostUrl_ExpectedSaveSiteMapResultsDataAsync(
            IList<PageInfo> crawlerResults,
            IList<PageInfo> siteMapResults,
            IList<PageInfo> expectedOnlyInCrawlerButNotInSiteMap,
            IList<PageInfo> expectedOnlyInSiteMapButNotInCrawler,
            IList<PageInfo> compositeResult)
        {
            var mockDao = new Mock<IDao<PageInfo>>();
            IReportService reportService = new ReportService(mockDao.Object);
            mockDao.Setup(dao => dao.SaveSiteMapResultsDataAsync("https://site.com", siteMapResults))
                .ReturnsAsync(siteMapResults.Count);
            
            await reportService.AddSiteMapExplorerResultsAsync("https://site.com", siteMapResults);
            
            mockDao.Verify(dao => dao.SaveSiteMapResultsDataAsync("https://site.com", siteMapResults), () => Times.Exactly(1));
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
                new List<PageInfo>(),
                CrawlerResults,
                new List<PageInfo>(),
                CrawlerResults
            };
            
            yield return new object[]
            {
                new List<PageInfo>(),
                CrawlerResults,
                new List<PageInfo>(),
                CrawlerResults,
                CrawlerResults
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<PageInfo> CrawlerResults =>
            new List<PageInfo>
            {
                new(new StringLoadResult("link1")),
                new(new StringLoadResult("link2")),
                new(new StringLoadResult("link3")),
                new(new StringLoadResult("link4")),
                new(new StringLoadResult("link6")),
                new(new StringLoadResult("link7"))
            };

        private IEnumerable<PageInfo> OnlyInCrawlerButNotInSiteMap =>
            new List<PageInfo>
            {
                new(new StringLoadResult("link6")),
                new(new StringLoadResult("link7"))
            };

        private IEnumerable<PageInfo> SiteMapResults =>
            new List<PageInfo>
            {
                new(new StringLoadResult("link1")),
                new(new StringLoadResult("link2")),
                new(new StringLoadResult("link3")),
                new(new StringLoadResult("link4")),
                new(new StringLoadResult("link8")),
                new(new StringLoadResult("link9"))
            };
        
        private IEnumerable<PageInfo> OnlyInSiteMapButNotInCrawler =>
            new List<PageInfo>
            {
                new(new StringLoadResult("link8")),
                new(new StringLoadResult("link9"))
            };
        
        private IEnumerable<PageInfo> CompositeResult => 
            new List<PageInfo>
            {
                new(new StringLoadResult("link1")),
                new(new StringLoadResult("link2")),
                new(new StringLoadResult("link3")),
                new(new StringLoadResult("link4")),
                new(new StringLoadResult("link6")),
                new(new StringLoadResult("link7")),
                new(new StringLoadResult("link8")),
                new(new StringLoadResult("link9"))
            };
    }
}