/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private const int ELEMENTS_COUNT = 10;

        private static readonly List<int> collection = new List<int>();
        private static readonly ManualResetEventSlim event1 = new ManualResetEventSlim(true);
        private static readonly ManualResetEventSlim event2 = new ManualResetEventSlim(false);

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            // feel free to add your code

            Thread thread1 = new Thread(AddElements) { Name = "Thread 1", };
            Thread thread2 = new Thread(PrintElements) { Name = "Thread 2" };
            thread1.Start(ELEMENTS_COUNT);
            thread2.Start(ELEMENTS_COUNT);
            thread1.Join();
            thread2.Join();

            Console.WriteLine("Finished!");

            Console.ReadLine();
        }

        private static void AddElements(object elementsCount)
        {
            var count = (int)elementsCount;

            for (int i = 1; i <= count; i++)
            {
                event1.Wait();
                collection.Add(i);
                Console.WriteLine($"{Thread.CurrentThread.Name} Added element {i} to the collection");
                event1.Reset();
                event2.Set();
            }
        }

        private static void PrintElements(object elementsCount)
        {
            var count = (int)elementsCount;
            while (collection.Count < count)
            {
                event2.Wait();
                Console.Write($"{Thread.CurrentThread.Name} prints collection: ");
                Console.WriteLine($"[{string.Join(",", collection)}]");
                Console.WriteLine();
                event2.Reset();
                event1.Set();
            }
        }

    }
}
