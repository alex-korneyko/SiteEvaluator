namespace SiteEvaluator.Data
{
    public interface IHasContent
    {
        public string Content { get; set; }

        void ClearContent();
    }
}