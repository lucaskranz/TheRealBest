namespace TheRealBest.API.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TheRealBest.API.Localization;
using TheRealBest.API.Middleware;
using TheRealBest.API.Models;
using TheRealBest.Application.DTOs.Audit;
using TheRealBest.Application.DTOs.Matches;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Enums;
using TheRealBest.Scoring.Rules;

public class LocalizationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    private static readonly ResxTranslationService Translations = new();

    public static TheoryData<string> Locales => new() { "pt-BR", "en", "es" };

    [Theory]
    [MemberData(nameof(Locales))]
    public void ActionLabels_ExistForEveryScoringActionInSingularAndPlural(string locale)
    {
        foreach (var action in ActionCatalog.All)
        {
            Translations.TranslateAction(action.Key, plural: false, locale).Should().NotBe(action.Key, $"{action.Key} needs a {locale} label");
            Translations.TranslateAction(action.Key, plural: true, locale).Should().NotBe(action.Key, $"{action.Key} needs a {locale} plural");
        }
    }

    [Theory]
    [MemberData(nameof(Locales))]
    public void Positions_ExistForEveryPosition(string locale)
    {
        foreach (var position in Enum.GetValues<PlayerPosition>())
        {
            Translations.TranslatePosition(position.ToString(), locale).Should().NotBe(position.ToString());
        }
    }

    [Theory]
    [InlineData("tackle", false, "pt-BR", "Desarme")]
    [InlineData("tackle", true, "pt-BR", "Desarmes")]
    [InlineData("tackle", false, "en", "Tackle")]
    [InlineData("tackle", true, "es", "Entradas")]
    [InlineData("own_goal", false, "es", "Gol en propia puerta")]
    public void TranslateAction_UsesSatelliteResourcesPerLocale(string key, bool plural, string locale, string expected)
    {
        Translations.TranslateAction(key, plural, locale).Should().Be(expected);
    }

    [Fact]
    public void TranslateMessage_FormatsArgumentsAndFallsBackToKey()
    {
        Translations.TranslateMessage("PlayerNotFound", "en", "abc").Should().Be("Player abc not found.");
        Translations.TranslateMessage("UnknownKey", "en").Should().Be("UnknownKey");
        Translations.TranslateAction("unknown_action", plural: false, "es").Should().Be("unknown_action");
    }

    [Theory]
    [InlineData(null, "pt-BR")]
    [InlineData("", "pt-BR")]
    [InlineData("pt-BR", "pt-BR")]
    [InlineData("pt-PT,pt;q=0.9", "pt-BR")]
    [InlineData("en-US,en;q=0.9", "en")]
    [InlineData("es-419", "es")]
    [InlineData("fr-FR", "pt-BR")]
    [InlineData("fr-FR, es;q=0.8, en;q=0.7", "es")]
    [InlineData("en;q=0.2, es;q=0.9", "es")]
    [InlineData("es;q=0, en", "en")]
    public void ResolveLocale_HonorsQualityWeightsAndPrimaryLanguage(string? acceptLanguage, string expected)
    {
        LocalizationMiddleware.ResolveLocale(acceptLanguage).Should().Be(expected);
    }

    [Theory]
    [InlineData("pt-BR", "Partida {0} não encontrada.")]
    [InlineData("en-US", "Match {0} not found.")]
    [InlineData("es", "Partido {0} no encontrado.")]
    public async Task NotFound_MessageFollowsAcceptLanguage(string acceptLanguage, string template)
    {
        var id = Guid.NewGuid();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/matches/{id}");
        request.Headers.AcceptLanguage.ParseAdd(acceptLanguage);

        var response = await factory.CreateClient().SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentLanguage.Should().Contain(SupportedLocales.Resolve(acceptLanguage));
        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOpts);
        envelope!.Errors.Should().Equal(string.Format(template, id));
    }

    [Fact]
    public async Task AuditReceipt_LabelsAndPositionAreLocalized()
    {
        var client = factory.CreateClient();
        var matches = await client.GetFromJsonAsync<ApiResponse<List<MatchSummaryDto>>>("/api/v1/matches?seasonYear=2023&page=1&pageSize=1", JsonOpts);
        if (matches?.Data is not { Count: > 0 })
        {
            return; // banco sem dados: nada a verificar
        }

        var matchId = matches.Data[0].Id;
        var detail = await client.GetFromJsonAsync<ApiResponse<MatchDetailDto>>($"/api/v1/matches/{matchId}", JsonOpts);
        var playerId = detail!.Data!.PlayerPerformances.First(p => p.ScoreId is not null).PlayerId;

        var english = await GetReceiptAsync(client, matchId, playerId, "en");
        var spanish = await GetReceiptAsync(client, matchId, playerId, "es");

        foreach (var receipt in new[] { english, spanish })
        {
            receipt.PositiveActions.Concat(receipt.Penalties)
                .Should().OnlyContain(item => item.Label != item.ActionKey, "every stored action key must resolve to a label");
        }

        english.PlayerPositionLabel.Should().Be(Translations.TranslatePosition(english.PlayerPosition, "en"));
        spanish.PlayerPositionLabel.Should().Be(Translations.TranslatePosition(spanish.PlayerPosition, "es"));
    }

    private static async Task<MatchPerformanceReceiptDto> GetReceiptAsync(HttpClient client, Guid matchId, Guid playerId, string locale)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/audit/matches/{matchId}/players/{playerId}");
        request.Headers.AcceptLanguage.ParseAdd(locale);

        var response = await client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<ApiResponse<MatchPerformanceReceiptDto>>(JsonOpts))!.Data!;
    }
}
