using System;
using System.Collections.Generic;
using System.Text;
using SiteEvaluator.Html.Tags;

namespace SiteEvaluator.Html
{
    public class HtmlParseService : IHtmlParseService
    {
        public List<string> GetNodesAsStringsList<T>(string rawHtml) where T : HtmlNode, new()
        {
            var listEachNodeFullStrings = new List<string>();

            var sbRawHtml = new StringBuilder(rawHtml);

            bool found;
            var tag = new T();

            do
            {
                var startNodeTagIndex = sbRawHtml.ToString().IndexOf(tag.OpenNodeTag, StringComparison.InvariantCulture);
                if (startNodeTagIndex > -1)
                {
                    found = true;
                    var relativeCloseNodeTagIndex = sbRawHtml
                        .ToString()
                        .Substring(startNodeTagIndex)
                        .IndexOf(tag.CloseNodeTag, StringComparison.InvariantCulture);
                    if (relativeCloseNodeTagIndex > -1)
                    {
                        var nodeFullStringLength = relativeCloseNodeTagIndex + tag.CloseNodeTag.Length;
                        var nodeString = sbRawHtml.ToString().Substring(startNodeTagIndex, nodeFullStringLength);
                        listEachNodeFullStrings.Add(nodeString);
                        sbRawHtml.Remove(0, startNodeTagIndex + nodeFullStringLength);
                    }
                    else
                    {
                        listEachNodeFullStrings.Add(tag.OpenNodeTag);
                        sbRawHtml.Remove(0, startNodeTagIndex + tag.OpenNodeTag.Length);
                    }
                }
                else
                {
                    found = false;
                }
            } while (found);

            return listEachNodeFullStrings;
        }

        public string ExtractBodyNode(string rawHtml)
        {
            var allTagStrings = GetNodesAsStringsList<Body>(rawHtml);

            return allTagStrings.Count > 0 ? allTagStrings[0] : "";
        }

        public T? DeserializeToNode<T>(string tagFullString) where T : HtmlNode, new()
        {
            var tag = new T();

            if (!tagFullString.StartsWith(tag.OpenNodeTag) || !tagFullString.EndsWith(tag.CloseNodeTag))
            {
                return null;
            }

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var customAttributes = propertyInfo.GetCustomAttributes(typeof(HtmlNodeAttributeAttribute), false);

                foreach (var customAttribute in customAttributes)
                {
                    if (customAttribute is not HtmlNodeAttributeAttribute htmlTagAttribute) continue;

                    var attributeName = htmlTagAttribute.AttributeName == ""
                        ? propertyInfo.Name.ToLower()
                        : htmlTagAttribute.AttributeName.ToLower();

                    var attributeStartIndex = tagFullString
                        .IndexOf(attributeName + "=\"", StringComparison.InvariantCulture);

                    if (attributeStartIndex == -1)
                    {
                        return tag;
                    }

                    var attributeValueStartIndex = attributeStartIndex + attributeName.Length + 2;

                    var attributeValueEndIndex = tagFullString
                        .Substring(attributeValueStartIndex)
                        .IndexOf('"', StringComparison.InvariantCulture) - 1;

                    var attributeValue = tagFullString
                        .Substring(attributeValueStartIndex, attributeValueEndIndex + 1);

                    propertyInfo.SetValue(tag, attributeValue);
                }
            }

            return tag;
        }
    }
}