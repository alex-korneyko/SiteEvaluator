using System;

namespace SiteEvaluator.ConsoleUI.ConsoleXtend
{
    public class MessageWriteType
    {
        private readonly bool _newLine;

        public MessageWriteType(bool newLine)
        {
            _newLine = newLine;
        }

        public void Info(string value = "")
        {
            Print(value, _newLine, ConsoleColor.White);
        }
        
        public void Note(string value = "")
        {
            Print(value, _newLine, ConsoleColor.Cyan);
        }

        public void Comment(string value = "")
        {
            Print(value, _newLine, ConsoleColor.DarkGray);
        }

        public void Error(string value = "")
        {
            Print(value, _newLine, ConsoleColor.Red);
        }

        public void Warning(string value = "")
        {
            Print(value, _newLine, ConsoleColor.Yellow);
        }

        public void Success(string value = "")
        {
            Print(value, _newLine, ConsoleColor.Green);
        }
        
        private static void Print(string value, bool newLine, ConsoleColor consoleColor)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            
            if (newLine) 
                Console.WriteLine(value);
            else
                Console.Write(value);
            
            Console.ForegroundColor = defaultColor;
        }
    }
}