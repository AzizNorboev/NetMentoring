/*
* Study the code of this application to calculate the sum of integers from 0 to N, and then
* change the application code so that the following requirements are met:
* 1. The calculation must be performed asynchronously.
* 2. N is set by the user from the console. The user has the right to make a new boundary in the calculation process,
* which should lead to the restart of the calculation.
* 3. When restarting the calculation, the application should continue working without any failures.
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal class Program
{
    private static CancellationTokenSource _lastCancellationTokenSource;
    private static Task<long> _lastMainProcessTask;
    private static Task _lastContinueTask;

    /// <summary>
    /// The Main method should not be changed at all.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
        Console.WriteLine("Calculating the sum of integers from 0 to N.");
        Console.WriteLine("Use 'c' key to exit...");
        Console.WriteLine();

        Console.WriteLine("Enter N: ");

        var input = Console.ReadLine();
        while (input.Trim().ToUpper() != "C")
        {
            if (int.TryParse(input, out var n))
            {
                CalculateSum(n);
            }
            else
            {
                Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
                Console.WriteLine("Enter N: ");
            }

            input = Console.ReadLine();
        }

        Console.WriteLine("Press any key to continue");
        Console.ReadLine();
    }

    private async static Task CalculateSum(int n)
    {
        // todo: make calculation asynchronous
        if (_lastMainProcessTask != null && !_lastMainProcessTask.IsCompleted)
        {
            // We are going to start another one SUM process so we should cancel the SUM process for the previous task
            _lastCancellationTokenSource.Cancel();

            //We want to wait the previous task (cancellation process in our case) to complete in order all the necessary messages.
            await _lastContinueTask;
        }

        using (var cts = new CancellationTokenSource())
        {
            _lastCancellationTokenSource = cts;

            _lastMainProcessTask = Calculator.Calculate(n, cts.Token);

            _lastContinueTask = _lastMainProcessTask.ContinueWith(prevTask =>
            {
                if (prevTask.IsCanceled)
                {
                    Console.WriteLine($"N({n}) => Task CANCELED.");
                    return;
                }

                Console.WriteLine($"N({n}) => SUM = {prevTask.Result}.");
                Console.WriteLine("");
                Console.WriteLine("Enter N: ");
            }, TaskContinuationOptions.None);

            Console.WriteLine();
            Console.WriteLine($"The task for {n} started... Enter N to cancel the request:");
            await _lastContinueTask;
        }
    }
}
