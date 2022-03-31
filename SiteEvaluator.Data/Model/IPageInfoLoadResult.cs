namespace SiteEvaluator.Data.Model
{
    public interface IPageInfoLoadResult
    {
        string RequestedUrl { get; }
        string Content { get; set; }
        long ContentLoadTime { get; set; }
        long Size { get; set; }
    }
}