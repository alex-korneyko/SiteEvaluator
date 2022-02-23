using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteEvaluator.PageLoader
{
    public class ContentLoadResult : IEquatable<ContentLoadResult>
    {
        public ContentLoadResult(string pageUrl)
        {
            PageUrl = pageUrl.EndsWith('/') ? pageUrl : pageUrl + '/';
        }

        public string PageUrl { get; }

        public string Content { get; private set; } = string.Empty;

        public HttpStatusCode HttpStatusCode { get; set; }

        public long PageLoadTime { get; set; }

        public bool IsSuccess { get; set; } = true;

        public Exception? Exception { get; set; }

        public async Task ApplyHttpResponse(HttpResponseMessage httpResponseMessage)
        {
            HttpStatusCode = httpResponseMessage.StatusCode;
            Content = await httpResponseMessage.Content.ReadAsStringAsync();
        }

        public bool Equals(ContentLoadResult? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PageUrl == other.PageUrl;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContentLoadResult)obj);
        }

        public override int GetHashCode()
        {
            return PageUrl.GetHashCode();
        }

        public override string ToString()
        {
            return IsSuccess
                ? $"Page URL: {PageUrl}, Status code: {HttpStatusCode}, Page load time: {PageLoadTime}ms"
                : $"Page URL: {PageUrl}, Is success: {IsSuccess}, Error message: {Exception?.Message}";
        }
    }
}