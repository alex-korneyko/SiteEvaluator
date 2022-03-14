namespace SiteEvaluator.ConsoleUI.ConsoleXtend
{
    public static class ConsoleX
    {
        public static MessageWriteType Write => new (false);
        public static MessageWriteType WriteLine => new (true);
        public static MessageReadType ReadLine => new();
    }
}