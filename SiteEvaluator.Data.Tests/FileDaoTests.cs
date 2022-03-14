using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SiteEvaluator.ContentLoader;
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
            var fileDao = new FileDao<ContentLoadResult>();

            var fileName = fileDao.GetFileName(hostUrl, suffix);

            Assert.Equal(expected, fileName);
        }

        [Theory]
        [ClassData(typeof(ContentLoadResultsCollectionAndJsonStringsCollection))]
        public void ConvertToJsonStrings_ContentLoadResultsCollection_ExpectedJsonStringsCollection(
            IEnumerable<ContentLoadResult> contentLoadResults,
            IEnumerable<string> jsonSerializedResultsCollection)
        {
            var fileDao = new FileDao<ContentLoadResult>();

            var jsonSerializedResults = fileDao.ConvertToJsonStrings(contentLoadResults);

            Assert.Equal(jsonSerializedResultsCollection, jsonSerializedResults);
        }

        [Theory]
        [ClassData(typeof(ContentLoadResultsCollectionAndJsonStringsCollection))]
        public void ConvertToContentLoadResults_JsonStringsCollection_ExpectedContentLoadResultsCollection(
            IEnumerable<ContentLoadResult> contentLoadResults,
            IEnumerable<string> jsonSerializedResultsCollection)
        {
            var fileDao = new FileDao<ContentLoadResult>();

            var toContentLoadResults = fileDao.ConvertToContentLoadResults(jsonSerializedResultsCollection);

            Assert.Equal(contentLoadResults, toContentLoadResults);
        }

        [Theory]
        [ClassData(typeof(ContentLoadResultsCollectionAndJsonStringsCollection))]
        public async Task SaveToFileAsync_FileNameJsonStringsCollection_ExpectedSavedLinesCount(
            IEnumerable<ContentLoadResult> contentLoadResults,
            IEnumerable<string> jsonSerializedResultsCollection)
        {
            const string fileName = "testFile.result";
            var fileDao = new FileDao<ContentLoadResult>();

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
                new List<ContentLoadResult>
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

        ContentLoadResult result1 => new("https://site.com")
        {
            ContentType = "text/html",
            HttpStatusCode = HttpStatusCode.OK,
            PageLoadTime = 100
        };

        ContentLoadResult result2 => new("https://site.com")
        {
            ContentType = "text/html",
            HttpStatusCode = HttpStatusCode.NotFound,
            PageLoadTime = 150
        };

        string ResultString1 => "{\"PageUrl\":\"https://site.com/\",\"Content\":\"\",\"HttpStatusCode\":200," +
                                "\"ContentType\":\"text/html\",\"PageLoadTime\":100,\"Size\":0,\"IsSuccess\":true,\"Exception\":null}";

        string ResultString2 => "{\"PageUrl\":\"https://site.com/\",\"Content\":\"\",\"HttpStatusCode\":404," +
                                "\"ContentType\":\"text/html\",\"PageLoadTime\":150,\"Size\":0,\"IsSuccess\":true,\"Exception\":null}";
    }
}