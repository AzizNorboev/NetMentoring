/*
 * 1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are not finished.
 * Each Task should iterate from 1 to 1000 and print into the console the following string:
 * “Task #0 – {iteration number}”.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task1._100Tasks
{
    class Program
    {
        const int TaskAmount = 100;
        //Should be 1000
        const int MaxIterationsCount = 1000;

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
            Task.WaitAll(taskArr);
        }

        //Please name variables logically not just a,b,c,n,...
        private static Task[] CreateTasks(int count) 
            => Enumerable.Range(0, count).Select(i =>
            {
                var task = new Task(Iterate, i);
                task.Start();
                return task;
            }).ToArray();

        private static void Iterate(object numberOfTask)
        {
            //Be attentive you have to itterate from 1 to 1000 not from 0 to 999
            for (int i = 1; i <= MaxIterationsCount; i++)
            {
                Console.WriteLine($"Task #{numberOfTask} - {i}.");
            }
        }
    }
}
