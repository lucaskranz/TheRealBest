namespace TheRealBest.Infrastructure.Tests;

using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using TheRealBest.Infrastructure.ExternalApis.ApiFootball;

public class ApiFootballRequestPacerTests
{
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

    private ApiFootballRequestPacer CreatePacer(int requestsPerMinute = 10) =>
        new(Options.Create(new ApiFootballOptions { RequestsPerMinute = requestsPerMinute }), _time);

    [Fact]
    public void MinInterval_SpreadsRequestsAcrossTheMinute()
    {
        CreatePacer(10).MinInterval.Should().Be(TimeSpan.FromSeconds(6));
        CreatePacer(300).MinInterval.Should().Be(TimeSpan.FromMilliseconds(200));
    }

    [Fact]
    public async Task WaitTurn_SecondRequestWaitsForTheInterval()
    {
        var pacer = CreatePacer(10);

        await pacer.WaitTurnAsync(CancellationToken.None);
        var second = pacer.WaitTurnAsync(CancellationToken.None);

        second.IsCompleted.Should().BeFalse();
        _time.Advance(TimeSpan.FromSeconds(5));
        second.IsCompleted.Should().BeFalse();
        _time.Advance(TimeSpan.FromSeconds(1));
        await second.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Observe_ExhaustedMinuteWindow_PausesForAFullMinute()
    {
        var pacer = CreatePacer(300);
        await pacer.WaitTurnAsync(CancellationToken.None);

        pacer.Observe(Headers((ApiFootballRequestPacer.MinuteRemainingHeader, "0")));
        var next = pacer.WaitTurnAsync(CancellationToken.None);

        _time.Advance(TimeSpan.FromSeconds(59));
        next.IsCompleted.Should().BeFalse();
        _time.Advance(TimeSpan.FromSeconds(1));
        await next.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Observe_ExhaustedDailyQuota_StopsFurtherRequests()
    {
        var pacer = CreatePacer();

        pacer.Observe(Headers((ApiFootballRequestPacer.DailyRemainingHeader, "0")));
        var act = () => pacer.WaitTurnAsync(CancellationToken.None);

        pacer.DailyRemaining.Should().Be(0);
        (await act.Should().ThrowAsync<ApiFootballException>()).Which.Kind.Should().Be(ApiFootballErrorKind.DailyQuotaExceeded);
    }

    private static System.Net.Http.Headers.HttpResponseHeaders Headers(params (string Name, string Value)[] headers)
    {
        var response = new HttpResponseMessage();
        foreach (var (name, value) in headers)
        {
            response.Headers.Add(name, value);
        }

        return response.Headers;
    }
}
