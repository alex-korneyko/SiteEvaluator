#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Data.DataHandlers
{
    public class FileDataHandlerService : IDataHandlerService
    {
        private const string FileExtension = ".hostData";
        private readonly string _dataDirectoryName = Environment.CurrentDirectory + @"\Data\";

        public IEnumerable<TargetHost> GetAllAsync()
        {
            //TODO Need to implement
            return new List<TargetHost>();
        }
        
        public async Task<TargetHost> GetTargetHostAsync(Uri hostUri)
        {
            var fileName = GetFileName(hostUri.Host);

            var jsonStrings = await LoadFromFileAsync(fileName);

            var targetHost = ConvertToTargetHost(jsonStrings);

            return targetHost ?? new TargetHost(hostUri);
        }

        public async Task<TargetHost> SaveTargetHostAsync(TargetHost targetHost)
        {
            var fileName = GetFileName(targetHost.HostUrl);
            
            targetHost.PageInfos.ForEach(pageInfo => pageInfo.ClearContent());

            var serializedTargetHost = JsonSerializer.Serialize(targetHost);

            await SaveToFileAsync(fileName, serializedTargetHost);

            return targetHost;
        }

        public string GetFileName(string hostUrl)
        {
            if (string.IsNullOrEmpty(hostUrl))
                throw new ArgumentNullException(hostUrl);

            var fullName = new StringBuilder();

            if (!hostUrl.Contains("//"))
                return fullName.Append(hostUrl).Append(".").ToString();
            
            hostUrl = hostUrl.Split("//")[1];
            hostUrl = hostUrl.EndsWith("/") ? hostUrl[..^1] : hostUrl;

            return hostUrl == "" ? "" : fullName.Append(hostUrl).Append(FileExtension).ToString();
        }

        public string ConvertToJsonStrings(TargetHost targetHost)
        {
            return JsonSerializer.Serialize(targetHost);
        }

        public TargetHost? ConvertToTargetHost(string targetHostJsonString)
        {
            try
            {
                var targetHost = JsonSerializer.Deserialize<TargetHost>(targetHostJsonString);
                if (targetHost != null) 
                    return targetHost;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }

        public async Task SaveToFileAsync(string fileName, string serializedTargetHost)
        {
            var fullFilePath = _dataDirectoryName + fileName;
            if (!Directory.Exists(_dataDirectoryName)) 
                Directory.CreateDirectory(_dataDirectoryName);

            if (File.Exists(fullFilePath))
                File.Delete(fullFilePath);
            
            await using var fileStream = new StreamWriter(fullFilePath, true);

            await fileStream.WriteAsync(serializedTargetHost);
        }

        public async Task<string> LoadFromFileAsync(string fileName)
        {
            var fullFilePath = _dataDirectoryName + fileName;
            if (!Directory.Exists(_dataDirectoryName)) 
                Directory.CreateDirectory(_dataDirectoryName);
            
            if (!File.Exists(fullFilePath))
            {
                File.Create(fullFilePath);
                return string.Empty;
            }

            using var fileStream = new StreamReader(fullFilePath);

            return await fileStream.ReadToEndAsync();
        }
    }
}