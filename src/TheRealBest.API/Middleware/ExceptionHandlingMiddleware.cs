namespace TheRealBest.API.Middleware;

using System.Net;
using System.Text.Json;
using TheRealBest.API.Models;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Interfaces;

/// <summary>
/// Converte exceções não tratadas no envelope padrão, com mensagem no idioma da requisição.
/// Detalhes técnicos ficam só no log, nunca na resposta.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ITranslationService translations,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado durante o processamento da requisição: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, messageKey) = exception switch
        {
            ArgumentException or FormatException => (HttpStatusCode.BadRequest, "InvalidRequest"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "ResourceNotFound"),
            _ => (HttpStatusCode.InternalServerError, "InternalError"),
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(translations.TranslateMessage(messageKey, SupportedLocales.Current));
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
