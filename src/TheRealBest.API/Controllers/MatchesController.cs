namespace TheRealBest.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Matches;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Interfaces;

[ApiController]
[Route("api/v1/matches")]
public sealed class MatchesController(
    ITranslationService translations,
    IGetPagedMatchesUseCase getPagedMatchesUseCase,
    IGetMatchDetailUseCase getMatchDetailUseCase
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MatchSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatches(
        [FromQuery] int seasonYear = 2023,
        [FromQuery] Guid? competitionId = null,
        [FromQuery] Guid? teamId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var filter = new MatchFilterDto(seasonYear, competitionId, teamId, page, pageSize);
        var result = await getPagedMatchesUseCase.ExecuteAsync(filter, cancellationToken);

        var meta = new ApiMeta
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };

        return Ok(ApiResponse<IReadOnlyList<MatchSummaryDto>>.Ok(result.Items, meta));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MatchDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatchDetail(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var match = await getMatchDetailUseCase.ExecuteAsync(id, cancellationToken);
        if (match is null)
        {
            return NotFound(ApiResponse<object>.Fail(translations.TranslateMessage("MatchNotFound", SupportedLocales.Current, id)));
        }

        return Ok(ApiResponse<MatchDetailDto>.Ok(match, new ApiMeta()));
    }
}