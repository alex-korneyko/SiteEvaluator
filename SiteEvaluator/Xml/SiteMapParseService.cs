using System;
using System.IO;
using System.Xml.Serialization;

namespace SiteEvaluator.Xml
{
    public class SiteMapParseService : ISiteMapParseService
    {
        public SiteMap DeserializeToSiteMap(string xmlString)
        {
            var namespaceForUrlset = GetNamespaceForUrlSet(xmlString);
            
            var xmlSerializer = new XmlSerializer(typeof(SiteMap), namespaceForUrlset);

            using var stream = GetStream(xmlString);

            try
            {
                var deserialize = (SiteMap)xmlSerializer.Deserialize(stream)!;
                return deserialize;
            }
            catch (InvalidOperationException)
            {
                return new SiteMap();
            }
        }

        private static Stream GetStream(string value)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(value);
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static string GetNamespaceForUrlSet(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString)) return "";

            const string urlSetXmlns = "urlset xmlns=\"";
            
            var indexOfStartUrlSetXmlnsName = xmlString
                .IndexOf(urlSetXmlns, StringComparison.InvariantCulture) + urlSetXmlns.Length;

            var lengthUrlSetXmlnsName = xmlString
                .IndexOf('\"', indexOfStartUrlSetXmlnsName) - indexOfStartUrlSetXmlnsName;

            if (indexOfStartUrlSetXmlnsName > -1 && lengthUrlSetXmlnsName > 0)
            {
                return xmlString.Substring(indexOfStartUrlSetXmlnsName, lengthUrlSetXmlnsName);
            }

            return "";
        }
    }
}