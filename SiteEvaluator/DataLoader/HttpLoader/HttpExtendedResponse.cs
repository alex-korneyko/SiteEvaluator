using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SiteEvaluator.DataLoader.HttpLoader
{
    public class HttpExtendedResponse : HttpResponseMessage
    {
        private readonly HttpResponseMessage _httpResponseMessage;

        public HttpExtendedResponse(HttpResponseMessage httpResponseMessage, long loadingTime)
        {
            _httpResponseMessage = httpResponseMessage;
            LoadingTime = loadingTime;
            StatusCode = httpResponseMessage.StatusCode;
            Version = httpResponseMessage.Version;
            Content = httpResponseMessage.Content;
            ReasonPhrase = httpResponseMessage.ReasonPhrase;
            RequestMessage = httpResponseMessage.RequestMessage;
        }

        public long LoadingTime { get; }

        public new HttpResponseHeaders Headers => _httpResponseMessage.Headers;
        public new HttpResponseHeaders TrailingHeaders => _httpResponseMessage.TrailingHeaders;
        public new bool IsSuccessStatusCode => _httpResponseMessage.IsSuccessStatusCode;
    }
}