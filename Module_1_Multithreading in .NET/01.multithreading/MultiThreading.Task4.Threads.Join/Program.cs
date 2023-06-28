/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        private const string MAIN_THREAD_NAME = "MAIN";
        static readonly Semaphore semaphoreSlim = new Semaphore(1, 1);
        private const int NUMBER_OF_ITERATIONS = 10;
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            Thread.CurrentThread.Name = MAIN_THREAD_NAME;

            Console.WriteLine("Start of option a:");
            ProcessA(NUMBER_OF_ITERATIONS);
            
            Console.WriteLine();
            
            Console.WriteLine("Start of option b:");
            ProcessB(NUMBER_OF_ITERATIONS);
            
            Console.ReadLine();
        }

        public static void ProcessA(object state)
        {
            var currentStateValue = (int)state;
            var isCreatedThread = Thread.CurrentThread.Name != MAIN_THREAD_NAME;
            var newStateValue = isCreatedThread ? currentStateValue - 1 : currentStateValue;

            if (isCreatedThread)
            {
                Console.WriteLine($"Thread with ID={Thread.CurrentThread.ManagedThreadId} has state={currentStateValue}");
            }

            if (currentStateValue > 1)
            {
                Thread t = new Thread(ProcessA);
                t.Start(newStateValue);
                t.Join();
            }
        }

        public static void ProcessB(object state)
        {
            var currentStateValue = (int)state;
            var isCreatedThread = Thread.CurrentThread.Name != MAIN_THREAD_NAME;
            var newStateValue = isCreatedThread ? currentStateValue -1 : currentStateValue;
            var canContinue = currentStateValue > 1;

            if (isCreatedThread)
            {
                if (canContinue)
                {
                    semaphoreSlim.WaitOne();
                }
                Console.WriteLine($"Thread with ID={Thread.CurrentThread.ManagedThreadId} has state={currentStateValue}");
            }

            if (canContinue)
            {
                ThreadPool.QueueUserWorkItem(ProcessB, newStateValue);
                if (isCreatedThread)
                {
                    semaphoreSlim.Release();
                }
            }
        }
    }
}

