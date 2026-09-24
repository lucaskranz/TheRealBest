namespace TheRealBest.Application.Tests;

using FluentAssertions;
using TheRealBest.Application.Localization;
using Xunit;

public class ActionLabelResolverTests
{
    private readonly ActionLabelResolver _labels = TestLabels.Create();

    [Theory]
    [InlineData(1, "en:tackle")]
    [InlineData(2, "en:tackle_plural")]
    [InlineData(0.45, "en:tackle_plural")] // métricas fracionárias (xA) usam o plural
    public void ActionLabel_UsesSingularOnlyForExactlyOne(decimal count, string expected)
    {
        _labels.ActionLabel("tackle", count, "en").Should().Be(expected);
    }

    [Theory]
    [InlineData("pt-PT", "pt-BR")]
    [InlineData("en-GB", "en")]
    [InlineData("es-MX", "es")]
    [InlineData("de", "pt-BR")]
    [InlineData(null, "pt-BR")]
    public void SupportedLocales_ResolveByPrimaryLanguage(string? tag, string expected)
    {
        SupportedLocales.Resolve(tag).Should().Be(expected);
    }
}
