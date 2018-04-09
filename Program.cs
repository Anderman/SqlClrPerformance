using System;

namespace testApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bm = new Benchmarks(Console.WriteLine);
            bm.Run();
        }
    }
}