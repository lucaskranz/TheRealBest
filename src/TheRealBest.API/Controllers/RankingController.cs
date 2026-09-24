namespace TheRealBest.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Ranking;
using TheRealBest.Application.Interfaces;
using TheRealBest.Domain.Enums;

[ApiController]
[Route("api/v1/ranking")]
public sealed class RankingController(IGetSeasonRankingUseCase getSeasonRankingUseCase) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SeasonRankingItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRankings(
        [FromQuery] int seasonYear = 2023,
        [FromQuery] PlayerPosition? position = null,
        [FromQuery] string? nationality = null,
        [FromQuery] bool onlyEligible = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var filter = new RankingFilterDto(seasonYear, position, nationality, onlyEligible, page, pageSize, search);
        var result = await getSeasonRankingUseCase.ExecuteAsync(filter, cancellationToken);

        var meta = new ApiMeta
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };

        return Ok(ApiResponse<IReadOnlyList<SeasonRankingItemDto>>.Ok(result.Items, meta));
    }

    [HttpGet("top")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SeasonRankingItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopContenders(
        [FromQuery] int seasonYear = 2023,
        [FromQuery] int count = 5,
        CancellationToken cancellationToken = default)
    {
        var items = await getSeasonRankingUseCase.GetTopContendersAsync(seasonYear, count, cancellationToken);
        var meta = new ApiMeta
        {
            Page = 1,
            PageSize = items.Count,
            TotalCount = items.Count,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false
        };

        return Ok(ApiResponse<IReadOnlyList<SeasonRankingItemDto>>.Ok(items, meta));
    }
}