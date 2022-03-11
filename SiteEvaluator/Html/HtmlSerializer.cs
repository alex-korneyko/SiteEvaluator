using System;
using System.Collections.Generic;
using System.Text;
using SiteEvaluator.Html.Tags;

namespace SiteEvaluator.Html
{
    public static class HtmlSerializer
    {
        public static List<string> GetAllTagFullStrings<T>(string rawHtml) where T : HtmlTag, new()
        {
            var listEachTagFullStrings = new List<string>();

            var sbRawHtml = new StringBuilder(rawHtml);

            bool found;
            var tag = new T();

            do
            {
                var startTagIndex = sbRawHtml.ToString().IndexOf(tag.OpenTag, StringComparison.InvariantCulture);
                if (startTagIndex > -1)
                {
                    found = true;
                    var relativeCloseTagIndex = sbRawHtml
                        .ToString()
                        .Substring(startTagIndex)
                        .IndexOf(tag.CloseTag, StringComparison.InvariantCulture);
                    if (relativeCloseTagIndex > -1)
                    {
                        var tagFullStringLength = relativeCloseTagIndex + tag.CloseTag.Length;
                        var tagString = sbRawHtml.ToString().Substring(startTagIndex, tagFullStringLength);
                        listEachTagFullStrings.Add(tagString);
                        sbRawHtml.Remove(0, startTagIndex + tagFullStringLength);
                    }
                    else
                    {
                        listEachTagFullStrings.Add(tag.OpenTag);
                        sbRawHtml.Remove(0, startTagIndex + tag.OpenTag.Length);
                    }
                }
                else
                {
                    found = false;
                }
            } while (found);

            return listEachTagFullStrings;
        }

        public static string GetBody(string rawHtml)
        {
            var allTagStrings = GetAllTagFullStrings<Body>(rawHtml);

            return allTagStrings.Count > 0 ? allTagStrings[0] : "";
        }

        public static T? Deserialize<T>(string tagFullString) where T : HtmlTag, new()
        {
            var tag = new T();

            if (!tagFullString.StartsWith(tag.OpenTag) || !tagFullString.EndsWith(tag.CloseTag))
            {
                return null;
            }

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var customAttributes = propertyInfo.GetCustomAttributes(typeof(HtmlTagAttributeAttribute), false);

                foreach (var customAttribute in customAttributes)
                {
                    if (customAttribute is not HtmlTagAttributeAttribute htmlTagAttribute) continue;

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