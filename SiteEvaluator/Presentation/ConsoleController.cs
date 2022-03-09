using System;
using static System.ConsoleColor;

namespace SiteEvaluator.Presentation
{
    public static class ConsoleController
    {
        public static MessageWriteType Write => new (false);
        public static MessageWriteType WriteLine => new (true);
        public static MessageReadType ReadLine => new();

        public class MessageWriteType
        {
            private readonly bool _newLine;

            public MessageWriteType(bool newLine)
            {
                _newLine = newLine;
            }

            public void Info(string value)
            {
                Print(value, _newLine, White);
            }
            
            public void Note(string value)
            {
                Print(value, _newLine, Cyan);
            }

            public void Comment(string value)
            {
                Print(value, _newLine, DarkGray);
            }

            public void Error(string value)
            {
                Print(value, _newLine, Red);
            }

            public void Warning(string value)
            {
                Print(value, _newLine, Yellow);
            }

            public void Success(string value)
            {
                Print(value, _newLine, Green);
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
        
        public class MessageReadType
        {
            public string Info(string value)
            {
                return Read(value, White);
            }
            
            public string Note(string value)
            {
                return Read(value, Cyan);
            }

            public string Success(string value)
            {
                return Read(value, Green);
            }

            public string Comment(string value)
            {
                return Read(value, DarkGray);
            }
            
            public string Warning(string value)
            {
                return Read(value, Yellow);
            }

            public string Error(string value)
            {
                return Read(value, Red);
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
}