
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSync
{
    public static class  ConsoleMsg
    {
        public static void Warning(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message, args);
            Console.WriteLine();
        }

        public static void Information(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message, args);
            Console.WriteLine();
        }

        public static void Highlight(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message, args);
            Console.WriteLine();
        }

        public static void Error(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message, args);
        }
    }
}
