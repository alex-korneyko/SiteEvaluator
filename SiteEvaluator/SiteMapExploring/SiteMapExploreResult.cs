using System;
using SiteEvaluator.Xml;

namespace SiteEvaluator.SiteMapExploring
{
    public class SiteMapExploreResult
    {
        public SiteMap SiteMap { get; } = new();

        public bool IsSuccess { get; }

        public Exception? Exception { get; }

        public string ErrorMessage { get; set; } = string.Empty;

        public SiteMapExploreResult(SiteMap siteMap)
        {
            SiteMap = siteMap;
            IsSuccess = true;
        }

        public SiteMapExploreResult(Exception? exception)
        {
            Exception = exception;
            IsSuccess = false;
        }

        public SiteMapExploreResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsSuccess = false;
        }
    }
}