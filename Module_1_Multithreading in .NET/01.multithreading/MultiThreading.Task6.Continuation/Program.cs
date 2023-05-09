/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            // Create a parent task
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<int> parentTask = Task.Run(() =>
            {
                int countdown = 4;
                while (!cts.IsCancellationRequested)
                {
                    countdown -= 1;
                    Console.WriteLine($"Parent task started: {countdown}...");
                    Thread.Sleep(1000);
                }
                // Throw an exception to simulate a failed task: click "continue" to execute case "b" and "c"
                throw new Exception("Parent task failed."); // comment this line to stimulate case "d"

                // Stimulate case "d" 
                cts.Token.ThrowIfCancellationRequested(); //comment this line to stimulate case "a"
                return 16042023;
            }, cts.Token);

            // a. Continuation task should be executed regardless of the result of the parent task.
            parentTask.ContinueWith(task =>
            {
                Console.WriteLine("Continuation task a executed regardless of parent task result. Parent result: " + parentTask.Result);
            });

            // b. Continuation task should be executed when the parent task finished without success.
            parentTask.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("Continuation task b executed because parent task finished without success. Parent exception: " + task.Exception.InnerException.Message);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            // c. Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.
            parentTask.ContinueWith(task =>
            {
                Console.WriteLine("Continuation task c executed when parent task is finished with fail and parent task thread should be reused for continuation. Parent exception: " + task.Exception.InnerException.Message);
            }, CancellationToken.None, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);

            // d. Continuation task should be executed outside of the thread pool when the parent task would be cancelled.
            parentTask.ContinueWith(task =>
            {
                Console.WriteLine("Continuation task d executed outside of the thread pool when the parent task would be cancelled.");
            }, CancellationToken.None, TaskContinuationOptions.LongRunning | TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.Default);

            // Cancel the parent task after 3 seconds
            Task.Delay(3000).ContinueWith(task =>
            {
                cts.Cancel();
                Console.WriteLine("Parent task cancelled.");
            });

            Console.ReadLine();
        }
    }
}
