namespace TheRealBest.Application;

using Microsoft.Extensions.DependencyInjection;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IIngestMatchDataUseCase, IngestMatchDataUseCase>();
        services.AddScoped<IRecalculateSeasonRankingUseCase, RecalculateSeasonRankingUseCase>();
        services.AddScoped<IMatchDataIngestionPipeline, MatchDataIngestionPipeline>();

        return services;
    }
}