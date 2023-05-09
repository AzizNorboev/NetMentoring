/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static List<int> _sharedCollection = new List<int>();
        private static bool _isAddingCompleted = false;

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();



            var task1 = Task.Run(() =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    _sharedCollection.Add(i);
                    Thread.Sleep(1000); //This is optional
                }

                _isAddingCompleted = true;

            });

            var task2 = Task.Run(() =>
            {
                int nextPrintedEkementIndex = 0;
                while (_isAddingCompleted == false || nextPrintedEkementIndex < _sharedCollection.Count)
                {
                    if (nextPrintedEkementIndex < _sharedCollection.Count)
                    {
                        for (int j = 0; j <= nextPrintedEkementIndex; j++)
                        {
                            Console.Write($"{_sharedCollection[j]}");
                            if (j < nextPrintedEkementIndex)
                            {
                                Console.Write(", ");
                            }
                        }
                        Console.WriteLine();

                        nextPrintedEkementIndex++;
                    }
                }
            });

            Task.WaitAll(task2);

            Console.ReadLine();
        }
    }
}
