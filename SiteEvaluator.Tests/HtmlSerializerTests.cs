using System;
using SiteEvaluator.Html;
using SiteEvaluator.Html.Tags;
using Xunit;

namespace SiteEvaluator.Tests
{
    public class HtmlSerializerTests
    {
        [Fact]
        public void GetAllTagFullStrings_RawHtmlString_TagStringsList()
        {
            var allTagFullStrings = HtmlSerializer.GetAllTagFullStrings<Body>(GetRawHtmlString());

            Assert.NotEmpty(allTagFullStrings);
            Assert.Collection(allTagFullStrings, tagString =>
            {
                Assert.NotNull(tagString);
                Assert.StartsWith("<body", tagString);
                Assert.EndsWith("</body>", tagString);
            });
        }

        [Fact]
        public void GetBody_RawHtmlString_ShouldReturnBodyString()
        {
            var body = HtmlSerializer.GetBody(GetRawHtmlString());

            Assert.Equal(GetBodyString(), body);
        }

        [Fact]
        public void Deserialize_BodyString_ShouldReturnBodyHtmlTag()
        {
            var body = HtmlSerializer.Deserialize<Body>(GetBodyString());

            Assert.NotNull(body);
            Assert.IsType<Body>(body);
            Assert.Equal("<body", body.OpenTag);
            Assert.Equal("</body>", body.CloseTag);
        }

        [Fact]
        public void Deserialize_AString_ShouldReturnAHtmlTag()
        {
            var a = HtmlSerializer.Deserialize<A>(GetAString());

            Assert.NotNull(a);
            Assert.IsType<A>(a);
            Assert.Equal("<a", a.OpenTag);
            Assert.Equal("</a>", a.CloseTag);
            Assert.NotNull(a.Href);
            Assert.Equal("https://i.ua", a.Href);
            Assert.NotNull(a.Rel);
            Assert.Equal("nofollow", a.Rel);
        }

        private string GetRawHtmlString()
        {
            return "" +
                   "<html>" +
                   "<head>" +
                   "</head>" +
                   GetBodyString() +
                   "</html>";
        }

        private string GetBodyString()
        {
            return "<body>" +
                   "<h1>Hello World!</h1>" +
                   GetAString() +
                   "</body>";
        }

        private string GetAString()
        {
            return "<a href=\"https://i.ua\" rel=\"nofollow\">some link</a>";
        }
    }
}