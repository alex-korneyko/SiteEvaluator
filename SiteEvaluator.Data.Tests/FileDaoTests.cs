using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SiteEvaluator.Data.DataHandlers;
using SiteEvaluator.Data.Model;
using SiteEvaluator.DataLoader;
using Xunit;

namespace SiteEvaluator.Data.Tests
{
    public class FileDaoTests
    {
        [Theory]
        [InlineData("https://aaa.com", ".crawler", @"aaa.com.crawler.result")]
        [InlineData("https://aaa.com/", ".crawler", @"aaa.com.crawler.result")]
        [InlineData("http://aaa.com", ".crawler", @"aaa.com.crawler.result")]
        [InlineData("aaa.com", ".crawler", @"aaa.com.crawler.result")]
        [InlineData("http://aaa.com", "", @"aaa.com.result")]
        [InlineData("http://", ".crawler", "")]
        [InlineData("", ".crawler", "")]
        public void GetFileName_HostUrlAndSuffix_ShouldReturnString(string hostUrl, string suffix, string expected)
        {
            var fileDao = new FileDataHandlerService<PageInfo>();

            var fileName = fileDao.GetFileName(hostUrl, suffix);

            Assert.Equal(expected, fileName);
        }

        [Theory]
        [ClassData(typeof(ContentLoadResultsCollectionAndJsonStringsCollection))]
        public void ConvertToJsonStrings_ContentLoadResultsCollection_ExpectedJsonStringsCollection(
            IEnumerable<PageInfo> pages,
            IEnumerable<string> jsonSerializedResultsCollection)
        {
            var fileDao = new FileDataHandlerService<PageInfo>();

            var jsonSerializedResults = fileDao.ConvertToJsonStrings(pages);

            Assert.Equal(jsonSerializedResultsCollection, jsonSerializedResults);
        }

        [Theory]
        [ClassData(typeof(ContentLoadResultsCollectionAndJsonStringsCollection))]
        public void ConvertToContentLoadResults_JsonStringsCollection_ExpectedContentLoadResultsCollection(
            IEnumerable<PageInfo> contentLoadResults,
            IEnumerable<string> jsonSerializedResultsCollection)
        {
            var fileDao = new FileDataHandlerService<PageInfo>();

            var toContentLoadResults = fileDao.ConvertToTargetHost(jsonSerializedResultsCollection);

            Assert.Equal(contentLoadResults, toContentLoadResults);
        }

        [Theory]
        [ClassData(typeof(ContentLoadResultsCollectionAndJsonStringsCollection))]
        public async Task SaveToFileAsync_FileNameJsonStringsCollection_ExpectedSavedLinesCount(
            IEnumerable<PageInfo> contentLoadResults,
            IEnumerable<string> jsonSerializedResultsCollection)
        {
            const string fileName = "testFile.result";
            var fileDao = new FileDataHandlerService<PageInfo>();

            var jsonResultsList = jsonSerializedResultsCollection.ToList();

            var savedStringsCount = await fileDao.SaveToFileAsync(fileName, jsonResultsList);

            var fullFilePath = Environment.CurrentDirectory + @"\Data\" + fileName;
            var fileStream = new FileStream(fullFilePath, FileMode.Open);

            Assert.True(File.Exists(fullFilePath));
            Assert.Equal(2, savedStringsCount);
            Assert.True(new StringBuilder().AppendJoin("", jsonResultsList).Length <= fileStream.Length);
        }
    }

    class ContentLoadResultsCollectionAndJsonStringsCollection : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<PageInfo>
                {
                    result1,
                    result2
                },
                new List<string>
                {
                    ResultString1,
                    ResultString2
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        PageInfo result1 => new (new StringLoadResult("https://site.com")
        {
            ContentType = "text/html",
            HttpStatusCode = HttpStatusCode.OK,
            ContentLoadTime = 100
        }, "https://site.com", ScannerType.SiteCrawler);

        PageInfo result2 => new(new StringLoadResult("https://site.com")
        {
            ContentType = "text/html",
            HttpStatusCode = HttpStatusCode.NotFound,
            ContentLoadTime = 150
        }, "https://site.com", ScannerType.SiteCrawler);

        string ResultString1 => "{\"Id\":0,\"SourceHost\":\"https://site.com\",\"ScannerType\":0," +
                                "\"Url\":\"https://site.com\",\"Content\":\"\",\"PageInfoUrls\":[]," +
                                "\"TotalLoadTime\":100,\"Level\":0,\"TotalSize\":0}";
        string ResultString2 => "{\"Id\":0,\"SourceHost\":\"https://site.com\",\"ScannerType\":0," +
                                "\"Url\":\"https://site.com\",\"Content\":\"\",\"PageInfoUrls\":[]," +
                                "\"TotalLoadTime\":150,\"Level\":0,\"TotalSize\":0}";
    }
}