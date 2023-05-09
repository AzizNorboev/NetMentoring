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
using System.Diagnostics;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static Semaphore semaphoreSlim = new Semaphore(1, 1);
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();


            // Option a)
            ThreadWithStateA tws = new ThreadWithStateA(10);

            Thread t = new Thread(tws.Process);
            t.Start();
            t.Join();
            // End of option a)

            // Option b)
            Console.WriteLine("Start of option b)");
            var stateB = 10;
            ThreadPool.QueueUserWorkItem(ProcessB, stateB);
            // End of option b)

            Console.ReadLine();
        }
        public static void ProcessB(object state)
        {
            semaphoreSlim.WaitOne();
            Thread.Sleep(100);
            Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}, State before decrement: {state}");
            state = (int)state - 1;
            Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}, State after decrement: {state}");

            if ((int)state > 1)
            {

                ThreadPool.QueueUserWorkItem(ProcessB, state);

            }
            semaphoreSlim.Release();
        }
    }

    public class ThreadWithStateA
    {
        private int _state;

        // The constructor obtains the state information.
        public ThreadWithStateA(int state)
        {
            _state = state;
        }

        public void Process()
        {
            Thread.Sleep(100);
            Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}, State before decrement: {_state}");
            _state = _state - 1;
            Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}, State after decrement: {_state}");

            if (_state > 1)
            {
                ThreadWithStateA tws = new ThreadWithStateA(_state);
                Thread t = new Thread(tws.Process);
                t.Start();
                t.Join();
            }

        }
    }
}

