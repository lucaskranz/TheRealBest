namespace TheRealBest.Application.UseCases;

using TheRealBest.Application.DTOs.Players;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Enums;
using TheRealBest.Domain.Interfaces;

public sealed class SearchPlayersUseCase(IPlayerRepository playerRepository, ActionLabelResolver labels) : ISearchPlayersUseCase
{
    public async Task<IReadOnlyList<PlayerSummaryDto>> ExecuteAsync(string? query, PlayerPosition? position, int limit = 20, CancellationToken cancellationToken = default)
    {
        var clampedLimit = Math.Clamp(limit, 1, 100);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var searchResults = await playerRepository.SearchAsync(query.Trim(), clampedLimit, cancellationToken);
            if (position.HasValue)
            {
                searchResults = searchResults.Where(p => p.PrimaryPosition == position.Value).ToList();
            }

            return searchResults.Select(p => new PlayerSummaryDto(
                Id: p.Id,
                Name: p.Name,
                Nationality: p.Nationality,
                PrimaryPosition: p.PrimaryPosition.ToString(),
                PrimaryPositionLabel: labels.PositionLabel(p.PrimaryPosition.ToString(), SupportedLocales.Current),
                PhotoUrl: p.PhotoUrl,
                DateOfBirth: p.DateOfBirth
            )).ToList();
        }

        if (position.HasValue)
        {
            var positionResults = await playerRepository.GetByPositionAsync(position.Value, cancellationToken);
            return positionResults.Take(clampedLimit).Select(p => new PlayerSummaryDto(
                Id: p.Id,
                Name: p.Name,
                Nationality: p.Nationality,
                PrimaryPosition: p.PrimaryPosition.ToString(),
                PrimaryPositionLabel: labels.PositionLabel(p.PrimaryPosition.ToString(), SupportedLocales.Current),
                PhotoUrl: p.PhotoUrl,
                DateOfBirth: p.DateOfBirth
            )).ToList();
        }

        // Se query e position forem nulos, pesquisa vazia retorna primeiros jogadores
        var defaultResults = await playerRepository.SearchAsync(string.Empty, clampedLimit, cancellationToken);
        return defaultResults.Select(p => new PlayerSummaryDto(
            Id: p.Id,
            Name: p.Name,
            Nationality: p.Nationality,
            PrimaryPosition: p.PrimaryPosition.ToString(),
            PrimaryPositionLabel: labels.PositionLabel(p.PrimaryPosition.ToString(), SupportedLocales.Current),
            PhotoUrl: p.PhotoUrl,
            DateOfBirth: p.DateOfBirth
        )).ToList();
    }
}