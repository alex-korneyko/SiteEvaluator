namespace SiteEvaluator.Data.Model
{
    public interface IHasContent
    {
        public string Content { get; set; }

        void ClearContent();
    }
}