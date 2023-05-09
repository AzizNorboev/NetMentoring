/*
 * 1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are not finished.
 * Each Task should iterate from 1 to 1000 and print into the console the following string:
 * “Task #0 – {iteration number}”.
 */
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task1._100Tasks
{
    class Program
    {
        const int TaskAmount = 100;
        const int MaxIterationsCount = 10;

        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. Multi threading V1.");
            Console.WriteLine("1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are not finished.");
            Console.WriteLine("Each Task should iterate from 1 to 1000 and print into the console the following string:");
            Console.WriteLine("“Task #0 – {iteration number}”.");
            Console.WriteLine();

            HundredTasks();

            Console.ReadLine();
        }

        static void HundredTasks()
        {
            Task[] taskArr = CreateTasks(TaskAmount);

            foreach (var task in taskArr)
            {
                task.Start();
            }

            Task.WaitAll(taskArr);
        }

        private static Task[] CreateTasks(int n)
        {
            Task[] tasks = new Task[n];
            var result = Enumerable.Range(0, n).Select(i => new Task(Iterate, i)).ToArray();
            return result;
        }

        private static void Iterate(object numberOfTask)
        {
            for (int i = 0; i < MaxIterationsCount; i++)
            {
                Thread.Sleep(10);
                Console.WriteLine($"Task #{numberOfTask} - {i}");
            }
        }
    }
}
