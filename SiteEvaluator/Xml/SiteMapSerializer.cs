using System;
using System.IO;
using System.Xml.Serialization;

namespace SiteEvaluator.Xml
{
    public static class SiteMapSerializer
    {
        public static string Serialize()
        {
            throw new NotImplementedException();
        }

        public static SiteMap Deserialize(string xmlString)
        {
            var namespaceForUrlset = GetNamespaceForUrlset(xmlString);
            
            var xmlSerializer = new XmlSerializer(typeof(SiteMap), namespaceForUrlset);

            using var stream = GetStream(xmlString);

            try
            {
                var deserialize = (SiteMap)xmlSerializer.Deserialize(stream)!;
                return deserialize;
            }
            catch (InvalidOperationException e)
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

        private static string GetNamespaceForUrlset(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString)) return "";

            const string urlsetXmlns = "urlset xmlns=\"";
            
            var indexOfStartUrlsetXmlnsName = xmlString
                .IndexOf(urlsetXmlns, StringComparison.InvariantCulture) + urlsetXmlns.Length;

            var lengthUrlsetXmlnsName = xmlString
                .IndexOf('\"', indexOfStartUrlsetXmlnsName) - indexOfStartUrlsetXmlnsName;

            if (indexOfStartUrlsetXmlnsName > -1 && lengthUrlsetXmlnsName > 0)
            {
                return xmlString.Substring(indexOfStartUrlsetXmlnsName, lengthUrlsetXmlnsName);
            }

            return "";
        }
    }
}