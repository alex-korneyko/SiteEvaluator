namespace SiteEvaluator.Data.Model
{
    public class PageInfoUrl : IEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public PageInfoUrlType PageInfoUrlType { get; set; }
        public PageInfo PageInfo { get; set; }
        public int PageInfoId { get; set; }

        public PageInfoUrl()
        {
        }

        public PageInfoUrl(string url, PageInfoUrlType pageInfoUrlType)
        {
            Url = url;
            PageInfoUrlType = pageInfoUrlType;
        }
    }
}