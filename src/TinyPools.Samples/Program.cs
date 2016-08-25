using System;

namespace TinyPools.Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PrintHelp();
            var read = Console.ReadKey(true);
            Console.WriteLine();

            switch (read.KeyChar)
            {
                case '1':
                    ObjectPoolSample.Run();
                    break;

                case '2':
                    MemoryPoolSample.Run();
                    break;

                case '3':
                    WrongPollUsageSample.Run();
                    break;
            }

            Console.WriteLine("Sample completed. Press any key to exit.");
            Console.ReadKey();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Please select a sample to execute:");
            Console.WriteLine("1 - Object pool sample");
            Console.WriteLine("2 - Memory pool sample");
            Console.WriteLine("3 - Wrong pool usage sample");
        }
    }
}