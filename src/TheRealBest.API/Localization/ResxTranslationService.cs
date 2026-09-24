namespace TheRealBest.API.Localization;

using System.Globalization;
using System.Resources;
using TheRealBest.Application.Localization;
using TheRealBest.Domain.Interfaces;

/// <summary>
/// Textos estáticos a partir de Resources/*.resx. O arquivo neutro contém o pt-BR (padrão e fallback);
/// en e es são satélites. Chaves ausentes devolvem a própria chave, para nunca quebrar uma resposta.
/// </summary>
public sealed class ResxTranslationService : ITranslationService
{
    public const string PluralSuffix = "_plural";

    private static readonly ResourceManager ActionLabels = Create("ActionLabels");
    private static readonly ResourceManager Positions = Create("Positions");
    private static readonly ResourceManager Messages = Create("Messages");

    public string TranslateAction(string actionKey, bool plural, string locale)
    {
        var culture = CultureFor(locale);
        return (plural ? ActionLabels.GetString(actionKey + PluralSuffix, culture) : null)
            ?? ActionLabels.GetString(actionKey, culture)
            ?? actionKey;
    }

    public string TranslatePosition(string positionKey, string locale) =>
        string.IsNullOrEmpty(positionKey) ? positionKey : Positions.GetString(positionKey, CultureFor(locale)) ?? positionKey;

    public string TranslateMessage(string messageKey, string locale, params object[] args)
    {
        var culture = CultureFor(locale);
        var template = Messages.GetString(messageKey, culture) ?? messageKey;
        return args.Length == 0 ? template : string.Format(culture, template, args);
    }

    private static CultureInfo CultureFor(string locale) => CultureInfo.GetCultureInfo(SupportedLocales.Resolve(locale));

    private static ResourceManager Create(string name) =>
        new($"{typeof(ResxTranslationService).Assembly.GetName().Name}.Resources.{name}", typeof(ResxTranslationService).Assembly);
}
