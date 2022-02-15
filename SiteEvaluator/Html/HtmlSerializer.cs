using System.Text;
using SiteEvaluator.Html.Tags;

namespace SiteEvaluator.Html;

public static class HtmlSerializer
{
    public static List<string> GetAllTagStrings<T>(string rawHtml) where T : HtmlTag, new()
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
                var closeTagIndex = sbRawHtml.ToString().IndexOf(tag.CloseTag, StringComparison.InvariantCulture);
                if (closeTagIndex > -1)
                {
                    var tagFullStringLength = closeTagIndex - startTagIndex + tag.CloseTag.Length;
                    var tagString = sbRawHtml.ToString().Substring(startTagIndex, tagFullStringLength);
                    listEachTagFullStrings.Add(tagString);
                    sbRawHtml.Remove(startTagIndex, tagFullStringLength);
                }
                else
                {
                    listEachTagFullStrings.Add(tag.OpenTag);
                    sbRawHtml.Remove(startTagIndex, tag.OpenTag.Length);
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
        var allTagStrings = GetAllTagStrings<Body>(rawHtml);

        return allTagStrings.Count > 0 ? allTagStrings[0] : "";
    }

    public static string Serialize(HtmlTag htmlTag)
    {
        throw new NotImplementedException();
    }

    public static T? Deserialize<T>(string stringTagValue) where T : HtmlTag, new()
    {
        var tag = new T();

        if (!stringTagValue.StartsWith(tag.OpenTag) || !stringTagValue.EndsWith(tag.CloseTag))
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
                    
                var attributeStartIndex = stringTagValue
                    .IndexOf(attributeName + "=\"", StringComparison.InvariantCulture);
                var attributeValueStartIndex = attributeStartIndex + attributeName.Length + 2;
                    
                var attributeValueEndIndex = stringTagValue
                    .Substring(attributeValueStartIndex)
                    .IndexOf('"', StringComparison.InvariantCulture) - 1;

                var attributeValue = stringTagValue
                    .Substring(attributeValueStartIndex, attributeValueEndIndex + 1);

                propertyInfo.SetValue(tag, attributeValue);
            }
        }

        return tag;
    }
}