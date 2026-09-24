namespace TheRealBest.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Audit;
using TheRealBest.Application.Interfaces;

[ApiController]
[Route("api/v1/audit")]
public sealed class AuditController(IGetMatchAuditReceiptUseCase getMatchAuditReceiptUseCase) : ControllerBase
{
    [HttpGet("matches/{matchId:guid}/players/{playerId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MatchPerformanceReceiptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatchReceipt(
        Guid matchId,
        Guid playerId,
        CancellationToken cancellationToken = default)
    {
        var receipt = await getMatchAuditReceiptUseCase.ExecuteAsync(matchId, playerId, cancellationToken);
        if (receipt is null)
        {
            return NotFound(ApiResponse<object>.Fail($"Recibo auditÃ¡vel para partida '{matchId}' e atleta '{playerId}' nÃ£o foi encontrado."));
        }

        return Ok(ApiResponse<MatchPerformanceReceiptDto>.Ok(receipt, new ApiMeta()));
    }
}