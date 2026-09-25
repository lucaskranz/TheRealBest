namespace TheRealBest.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Comparison;
using TheRealBest.Application.Interfaces;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Interfaces;

[ApiController]
[Route("api/v1/comparison")]
public sealed class ComparisonController(
    IGetBallonDorComparisonUseCase getBallonDorComparisonUseCase,
    ITranslationService translations) : ControllerBase
{
    /// <summary>Classificação oficial da Bola de Ouro do ano contra o Fair Season Score da temporada avaliada.</summary>
    [HttpGet("ballon-dor/{year:int}")]
    [ProducesResponseType(typeof(ApiResponse<BallonDorComparisonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BallonDorComparisonDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBallonDor(int year, CancellationToken cancellationToken = default)
    {
        var comparison = await getBallonDorComparisonUseCase.ExecuteAsync(year, cancellationToken);
        return comparison is null
            ? NotFound(ApiResponse<BallonDorComparisonDto>.Fail(
                translations.TranslateMessage("BallonDorEditionNotFound", SupportedLocales.Current, year)))
            : Ok(ApiResponse<BallonDorComparisonDto>.Ok(comparison));
    }
}
