namespace TheRealBest.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TheRealBest.API.Formula;
using TheRealBest.API.Models;
using TheRealBest.Application.Localization;

[ApiController]
[Route("api/v1/formula")]
public sealed class FormulaController(FormulaDescriptorFactory factory) : ControllerBase
{
    /// <summary>Pesos, linhas de base e multiplicadores do algoritmo em vigor, com rótulos no idioma da requisição.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<FormulaDescriptor>), StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(ApiResponse<FormulaDescriptor>.Ok(factory.Create(SupportedLocales.Current)));
}
