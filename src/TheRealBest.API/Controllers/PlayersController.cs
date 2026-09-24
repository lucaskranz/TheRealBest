namespace TheRealBest.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Players;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Interfaces;
using TheRealBest.Domain.Enums;

[ApiController]
[Route("api/v1/players")]
public sealed class PlayersController(
    ITranslationService translations,
    IGetPlayerProfileUseCase getPlayerProfileUseCase,
    ISearchPlayersUseCase searchPlayersUseCase
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PlayerSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPlayers(
        [FromQuery] string? query = null,
        [FromQuery] PlayerPosition? position = null,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var items = await searchPlayersUseCase.ExecuteAsync(query, position, limit, cancellationToken);
        var meta = new ApiMeta
        {
            Page = 1,
            PageSize = items.Count,
            TotalCount = items.Count,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        return Ok(ApiResponse<IReadOnlyList<PlayerSummaryDto>>.Ok(items, meta));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PlayerProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayerProfile(
        Guid id,
        [FromQuery] int seasonYear = 2023,
        CancellationToken cancellationToken = default)
    {
        var profile = await getPlayerProfileUseCase.ExecuteAsync(id, seasonYear, cancellationToken);
        if (profile is null)
        {
            return NotFound(ApiResponse<object>.Fail(translations.TranslateMessage("PlayerNotFound", SupportedLocales.Current, id)));
        }

        return Ok(ApiResponse<PlayerProfileDto>.Ok(profile, new ApiMeta()));
    }
}