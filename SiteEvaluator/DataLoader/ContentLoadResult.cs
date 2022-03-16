using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SiteEvaluator.DataLoader.HttpLoader;

namespace SiteEvaluator.DataLoader
{
    public abstract class ContentLoadResult<T> 
        : IEquatable<ContentLoadResult<T>>, IComparable<ContentLoadResult<T>>
        where T : class
    {
        protected ContentLoadResult(string pageUrl)
        {
            PageUrl = pageUrl.EndsWith('/') ? pageUrl : pageUrl + '/';
        }

        public string PageUrl { get; }

        public T? Content { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public long ContentLoadTime { get; set; }

        public long Size { get; set; }

        public bool IsSuccess { get; set; } = true;

        public Exception? Exception { get; set; }

        public async Task ApplyHttpResponseAsync(HttpExtendedResponse httpExtendedResponseMessage)
        {
            HttpStatusCode = httpExtendedResponseMessage.StatusCode;
            
            Content = await ApplyContent(httpExtendedResponseMessage.Content);
            
            Size = (await httpExtendedResponseMessage.Content.ReadAsStreamAsync()).Length;
            
            var contentTypeHeaders = httpExtendedResponseMessage.Content.Headers
                .FirstOrDefault(header => header.Key.ToLower() == "content-type")
                .Value ?? new List<string>();

            var contentTypeBuilder = new StringBuilder().AppendJoin("; ", contentTypeHeaders);
            
            ContentType = contentTypeBuilder.ToString();

            ContentLoadTime = httpExtendedResponseMessage.LoadingTime;
        }

        public bool Equals(ContentLoadResult<T>? other)
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
            
            return Equals((ContentLoadResult<T>)obj);
        }
        
        public void ClearContent()
        {
            Content = null;
        }

        public override int GetHashCode()
        {
            return PageUrl.GetHashCode();
        }

        public override string ToString()
        {
            return IsSuccess
                ? $"Page URL: {PageUrl}, Status code: {HttpStatusCode}, Page load time: {ContentLoadTime}ms"
                : $"Page URL: {PageUrl}, Is success: {IsSuccess}, Error message: {Exception?.Message}";
        }

        public int CompareTo(ContentLoadResult<T>? other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            
            if (ReferenceEquals(null, other))
                return 1;
            
            return ContentLoadTime.CompareTo(other.ContentLoadTime);
        }

        protected abstract Task<T?> ApplyContent(HttpContent httpContent);
    }
}