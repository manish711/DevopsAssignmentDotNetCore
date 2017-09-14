using System;

namespace HelloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int x = 1;
            int y = 2;
            Console.WriteLine("Hello World");
            Program.Add(x, y);
            Console.Read();
        }

        public static void Add(int x, int y)
        {
            Console.WriteLine(x + y);
        }
    }
}