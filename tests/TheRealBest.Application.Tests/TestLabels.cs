namespace TheRealBest.Application.Tests;

using TheRealBest.Application.Localization;
using TheRealBest.Domain.Interfaces;

/// <summary>
/// Tradutor determinístico para testes: devolve "{locale}:{chave}" (e "_plural" no plural),
/// para as asserções verificarem que chave e forma foram escolhidas corretamente.
/// </summary>
public static class TestLabels
{
    public static ActionLabelResolver Create() => new(new EchoTranslationService());

    private sealed class EchoTranslationService : ITranslationService
    {
        public string TranslateAction(string actionKey, bool plural, string locale) =>
            $"{locale}:{actionKey}{(plural ? "_plural" : string.Empty)}";

        public string TranslatePosition(string positionKey, string locale) => $"{locale}:{positionKey}";

        public string TranslateMessage(string messageKey, string locale, params object[] args) => $"{locale}:{messageKey}";
    }
}
