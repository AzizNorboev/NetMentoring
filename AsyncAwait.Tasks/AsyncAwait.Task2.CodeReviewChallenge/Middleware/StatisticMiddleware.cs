using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;
    private static readonly ConcurrentDictionary<string, int> _unregisteredClicks = new ConcurrentDictionary<string, int>();

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        RegisterVisit(context);
        await UpdateHeaders(context);
        await _next(context);
    }

    private static string GetCurrentRequestUrlPath(HttpContext context)
        => context.Request.Path.ToString().ToLower();

    private async Task UpdateHeaders(HttpContext context)
    {
        string path = GetCurrentRequestUrlPath(context);
        var registeredCount = await _statisticService.GetVisitsCountAsync(path);
        _unregisteredClicks.TryGetValue(path, out var unregisteredClicksValue);
        context.Response.Headers.Add(CustomHttpHeaders.TotalPageVisits, $"{registeredCount + unregisteredClicksValue}");
    }

    private async void RegisterVisit(HttpContext context)
    {
        string path = GetCurrentRequestUrlPath(context);
        _unregisteredClicks.AddOrUpdate(path, 1, (_, currentValue) => { return currentValue + 1; });

        await _statisticService.RegisterVisitAsync(path);

        var unregisteredClicksValue = _unregisteredClicks.AddOrUpdate(path, 0, (_, currentValue) => { return currentValue - 1; });
        if (unregisteredClicksValue <= 0)
        {
            _unregisteredClicks.TryRemove(path, out _);
        }
    }
}
