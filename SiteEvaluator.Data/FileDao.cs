#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SiteEvaluator.Data
{
    public class FileDao<T> : IDao<T> where T : IHasContent
    {
        private const string FileNameCrawlerSuffix = ".crawler";
        private const string FileNameSiteMapSuffix = ".sitemap";
        private readonly string _dataDirectoryName = Environment.CurrentDirectory + @"\Data\";

        public async Task<IEnumerable<T>> GetCrawlerResultsData(string hostUrl)
        {
            var fileName = GetFileName(hostUrl, FileNameCrawlerSuffix);

            var jsonStrings = await LoadFromFileAsync(fileName);

            var contentLoadResults = ConvertToContentLoadResults(jsonStrings);

            return contentLoadResults;
        }

        public async Task<IEnumerable<T>> GetSiteMapResultsData(string hostUrl)
        {
            var fileName = GetFileName(hostUrl, FileNameSiteMapSuffix);

            var jsonStrings = await LoadFromFileAsync(fileName);

            var contentLoadResults = ConvertToContentLoadResults(jsonStrings);

            return contentLoadResults;
        }

        public async Task<long> SaveCrawlerResultsDataAsync(string hostUrl, IEnumerable<T> data)
        {
            var fileName = GetFileName(hostUrl, FileNameCrawlerSuffix);

            if (fileName == "")
                return 0;

            var jsonSerializedResults = ConvertToJsonStrings(data);

            var count = await SaveToFileAsync(fileName, jsonSerializedResults);
            
            return count;
        }

        public async Task<long> SaveSiteMapResultsDataAsync(string hostUrl, IEnumerable<T> data)
        {
            var fileName = GetFileName(hostUrl, FileNameSiteMapSuffix);

            if (fileName == "")
                return 0;

            var jsonSerializedResults = ConvertToJsonStrings(data);

            var count = await SaveToFileAsync(fileName, jsonSerializedResults);
            
            return count;
        }

        public string GetFileName(string hostUrl, string suffix)
        {
            if (string.IsNullOrEmpty(hostUrl))
                return "";

            var fullName = new StringBuilder();

            if (!hostUrl.Contains("//"))
                return fullName.Append(hostUrl).Append(suffix).Append(".result").ToString();
            
            hostUrl = hostUrl.Split("//")[1];
            hostUrl = hostUrl.EndsWith("/") ? hostUrl[..^1] : hostUrl;

            return hostUrl == "" ? "" : fullName.Append(hostUrl).Append(suffix).Append(".result").ToString();
        }

        public IEnumerable<string> ConvertToJsonStrings(IEnumerable<T> contentLoadResults)
        {
            var jsonDataSet = new List<string>();
            
            foreach (var contentLoadResult in contentLoadResults)
            {
                contentLoadResult.ClearContent();
                var serializedResult = JsonSerializer.Serialize(contentLoadResult);
                jsonDataSet.Add(serializedResult);
            }

            return jsonDataSet;
        }

        public IEnumerable<T> ConvertToContentLoadResults(IEnumerable<string> jsonStringsCollection)
        {
            var contentLoadResults = new List<T>();

            foreach (var jsonString in jsonStringsCollection)
            {
                var contentLoadResult = JsonSerializer.Deserialize<T>(jsonString);
                
                if (contentLoadResult != null) 
                    contentLoadResults.Add(contentLoadResult);
            }

            return contentLoadResults;
        }

        public async Task<long> SaveToFileAsync(string fileName, IEnumerable<string> jsonSerializedResults)
        {
            var fullFilePath = _dataDirectoryName + fileName;
            if (!Directory.Exists(_dataDirectoryName)) 
                Directory.CreateDirectory(_dataDirectoryName);

            if (File.Exists(fullFilePath))
                File.Delete(fullFilePath);

            long counter = 0;

            await using var fileStream = new StreamWriter(fullFilePath, true);
            
            foreach (var jsonSerializedResult in jsonSerializedResults)
            {
                await fileStream.WriteLineAsync(jsonSerializedResult);
                counter++;
            }

            return counter;
        }

        public async Task<IEnumerable<string>> LoadFromFileAsync(string fileName)
        {
            var fullFilePath = _dataDirectoryName + fileName;
            if (!Directory.Exists(_dataDirectoryName)) 
                Directory.CreateDirectory(_dataDirectoryName);
            
            var result = new List<string>();
            
            if (!File.Exists(fullFilePath))
                return result;

            using var fileStream = new StreamReader(fullFilePath);

            string? jsonLine;
            while ((jsonLine = await fileStream.ReadLineAsync()) != null)
            {
                result.Add(jsonLine);
            }

            return result;
        }
    }
}