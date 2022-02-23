using System;

namespace SiteEvaluator.Presentation
{
    public static class ConsoleMessage
    {
        public static void WriteLineError(string value)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(value);
            Console.ForegroundColor = defaultColor;
        }

        public static void WriteLineWarning(string value)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(value);
            Console.ForegroundColor = defaultColor;
        }

        public static void WriteLineSuccess(string value)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(value);
            Console.ForegroundColor = defaultColor;
        }
    }
}