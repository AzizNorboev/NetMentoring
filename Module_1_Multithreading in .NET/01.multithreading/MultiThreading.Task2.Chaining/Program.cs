/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static Random randNum = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            // feel free to add your code
            Task<int[]> taskCreate = new Task<int[]>(() => CreateArray(10));
            taskCreate.Start();
            taskCreate.Wait();
            Console.WriteLine("Array created:");
            DisplayArrayValues(taskCreate.Result);

            Task<int[]> taskMultiply = new Task<int[]>(() => Multiply(taskCreate.Result));
            taskMultiply.Start();
            taskMultiply.Wait();
            Console.WriteLine("Array multiplied by random number:");
            DisplayArrayValues(taskMultiply.Result);

            Task<int[]> taskSortAsc = new Task<int[]>(() => SortArray(taskMultiply.Result));
            taskSortAsc.Start();
            taskSortAsc.Wait();
            Console.WriteLine("Array sorted:");
            DisplayArrayValues(taskSortAsc.Result);

            Task<double> taskAverage = new Task<double>(() => GetAverage(taskSortAsc.Result));
            taskAverage.Start();
            taskAverage.Wait();
            Console.WriteLine("Average of array:");
            Console.WriteLine(taskAverage.Result);
            Console.ReadLine();
        }

        private static int[] CreateArray(int count)
        {
            int[] test2 = new int[count];

            for (int i = 0; i < test2.Length; i++)
            {
                test2[i] = randNum.Next(0, 99);
            }
            return test2;
        }

        private static int[] Multiply(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] *= randNum.Next(1, 10);
            }
            return array;
        }

        private static int[] SortArray(int[] array)
        {
            Array.Sort(array);
            return array;
        }

        private static double GetAverage(int[] array)
        {
            return (from num in array
                   select num).Average();
        }

        private static void DisplayArrayValues(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write($"{array[i]} ");
            }
            Console.WriteLine(Environment.NewLine);
        }
    }
}
