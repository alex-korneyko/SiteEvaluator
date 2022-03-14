using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SiteEvaluator.Data;

namespace SiteEvaluator.ContentLoader
{
    public class ContentLoadResult : IEquatable<ContentLoadResult>, IComparable<ContentLoadResult>, IHasContent
    {
        public ContentLoadResult(string pageUrl)
        {
            PageUrl = pageUrl.EndsWith('/') ? pageUrl : pageUrl + '/';
        }

        public string PageUrl { get; }

        public string Content { get; set; } = string.Empty;

        public HttpStatusCode HttpStatusCode { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public long PageLoadTime { get; set; }

        public long Size { get; set; }

        public bool IsSuccess { get; set; } = true;

        public Exception? Exception { get; set; }

        public async Task ApplyHttpResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            HttpStatusCode = httpResponseMessage.StatusCode;
            Content = await httpResponseMessage.Content.ReadAsStringAsync();
            Size = Content.Length;
            var contentTypeHeaders = httpResponseMessage.Content.Headers
                .FirstOrDefault(header => header.Key.ToLower() == "content-type").Value;

            if (contentTypeHeaders != null)
            {
                var stringBuilder = new StringBuilder().AppendJoin("; ", contentTypeHeaders);
                ContentType = stringBuilder.ToString();
            }
        }

        public bool Equals(ContentLoadResult? other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            
            if (ReferenceEquals(this, other)) 
                return true;
            
            return PageUrl == other.PageUrl;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            
            if (ReferenceEquals(this, obj)) 
                return true;
            
            if (obj.GetType() != GetType()) 
                return false;
            
            return Equals((ContentLoadResult)obj);
        }
        
        public void ClearContent()
        {
            Content = string.Empty;
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

        public int CompareTo(ContentLoadResult? other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            
            if (ReferenceEquals(null, other))
                return 1;
            
            return PageLoadTime.CompareTo(other.PageLoadTime);
        }
    }
}