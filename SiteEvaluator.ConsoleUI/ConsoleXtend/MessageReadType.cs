using System;

namespace SiteEvaluator.ConsoleUI.ConsoleXtend
{
    public class MessageReadType
    {
        public string Info(string value = "")
        {
            return Read(value, ConsoleColor.White);
        }
            
        public string Note(string value = "")
        {
            return Read(value, ConsoleColor.Cyan);
        }

        public string Success(string value = "")
        {
            return Read(value, ConsoleColor.Green);
        }

        public string Comment(string value = "")
        {
            return Read(value, ConsoleColor.DarkGray);
        }
            
        public string Warning(string value = "")
        {
            return Read(value, ConsoleColor.Yellow);
        }

        public string Error(string value = "")
        {
            return Read(value, ConsoleColor.Red);
        }

        private string Read(string value, ConsoleColor consoleColor)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.Write(value);
            var readLine = Console.ReadLine();
            Console.ForegroundColor = defaultColor;

            return readLine ?? "";
        }
    }
}