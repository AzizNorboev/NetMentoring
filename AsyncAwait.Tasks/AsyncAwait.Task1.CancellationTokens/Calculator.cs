using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal static class Calculator
{
    // todo: change this method to support cancellation token
    public static async Task<long> Calculate(int n, CancellationToken token)
    {
        long sum = 0;

        for (var i = 0; i < n; i++)
        {
            sum += i + 1; // i + 1 is to allow 2147483647 (Max(Int32)) 
            try
            {
                //Waiting for a long process
                await Task.Delay(10, token);
            }
            //Do not care about exceptions
            catch
            {
            }

            if (token.IsCancellationRequested)
            {
                OutputCalculationProcessCanceled(n);
                token.ThrowIfCancellationRequested();
            }
        }

        return sum;
    }

    private static void OutputCalculationProcessCanceled(int n)
    {
        Console.WriteLine($"N({n}) => Calculation process CANCELED.");
    }
}
