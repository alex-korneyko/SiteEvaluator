namespace SiteEvaluator.ConsoleUI
{
    public static class ConsoleApplication
    {
        public static ApplicationBuilder CreateBuilder(params string[] args)
        {
            return new ApplicationBuilder(args);
        }
    }
}