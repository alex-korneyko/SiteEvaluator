using System;
using System.Collections.Generic;
using System.Text;
using SiteEvaluator.Html.Nodes;

namespace SiteEvaluator.Html
{
    public class HtmlParseService : IHtmlParseService
    {
        public List<string> GetNodesAsStringsList<T>(string rawHtml) where T : HtmlNode, new()
        {
            var listEachNodeFullStrings = new List<string>();
            var sbRawHtml = new StringBuilder(rawHtml);

            for (;;)
            {
                var startNodeTagIndex = sbRawHtml
                    .ToString()
                    .IndexOf(new T().OpenNodeTag, StringComparison.InvariantCulture);
                
                if (startNodeTagIndex > -1)
                {
                    var nodeFullString = 
                        ExtractNodeFullStringByStartIndexAndRemoveItFromRawHtml<T>(sbRawHtml, startNodeTagIndex);
                    listEachNodeFullStrings.Add(nodeFullString);
                    continue;
                }
                break;
            }

            return listEachNodeFullStrings;
        }

        public string ExtractBodyNode(string rawHtml)
        {
            var allTagStrings = GetNodesAsStringsList<Body>(rawHtml);

            return allTagStrings.Count > 0 ? allTagStrings[0] : "";
        }

        public T? DeserializeToNode<T>(string nodeFullString) where T : HtmlNode, new()
        {
            var tag = new T();

            if (!nodeFullString.StartsWith(tag.OpenNodeTag) || !nodeFullString.EndsWith(tag.CloseNodeTag))
                return null;

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var customAttributes = propertyInfo.GetCustomAttributes(typeof(HtmlNodeAttributeAttribute), false);

                foreach (var customAttribute in customAttributes)
                {
                    if (customAttribute is not HtmlNodeAttributeAttribute htmlTagAttribute)
                        continue;

                    var attributeName = htmlTagAttribute.AttributeName == ""
                        ? propertyInfo.Name.ToLower()
                        : htmlTagAttribute.AttributeName.ToLower();

                    var attributeValue = GetAttributeValue(nodeFullString, attributeName);

                    propertyInfo.SetValue(tag, attributeValue);
                }
            }

            return tag;
        }
        
        private string GetAttributeValue(string nodeFullString, string attributeName)
        {
            var attributeStartIndex = nodeFullString
                .IndexOf(attributeName + "=\"", StringComparison.InvariantCulture);

            if (attributeStartIndex == -1)
                return "";

            var attributeValueStartIndex = attributeStartIndex + attributeName.Length + "\"=".Length;

            var attributeValueEndIndex = nodeFullString
                .Substring(attributeValueStartIndex)
                .IndexOf('"', StringComparison.InvariantCulture) - 1;

            return nodeFullString
                .Substring(attributeValueStartIndex, attributeValueEndIndex + 1);
        }
        
        private string ExtractNodeFullStringByStartIndexAndRemoveItFromRawHtml<T>(
            StringBuilder sbRawHtml,
            int startNodeTagIndex) 
            where T : HtmlNode, new()
        {
            var htmlNode = new T();

            var relativeCloseNodeTagIndex = sbRawHtml
                .ToString()
                .Substring(startNodeTagIndex)
                .IndexOf(htmlNode.CloseNodeTag, StringComparison.InvariantCulture);

            if (relativeCloseNodeTagIndex > -1)
            {
                var nodeFullStringLength = relativeCloseNodeTagIndex + htmlNode.CloseNodeTag.Length;
                var nodeString = sbRawHtml.ToString().Substring(startNodeTagIndex, nodeFullStringLength);
                sbRawHtml.Remove(0, startNodeTagIndex + nodeFullStringLength);
                return nodeString;
            }

            sbRawHtml.Remove(0, startNodeTagIndex + htmlNode.OpenNodeTag.Length);
            return htmlNode.OpenNodeTag;
        }
    }
}