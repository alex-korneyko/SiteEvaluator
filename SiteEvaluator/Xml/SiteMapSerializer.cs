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
            var xmlSerializer = new XmlSerializer(typeof(SiteMap));

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
    }
}