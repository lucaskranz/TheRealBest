namespace TheRealBest.Application.Localization;

using TheRealBest.Domain.Interfaces;

/// <summary>
/// Rótulos localizados do recibo e das posições. O recibo guarda só as chaves (ActionScoreItem.ActionKey);
/// o texto é resolvido na leitura, no idioma da requisição.
/// </summary>
public sealed class ActionLabelResolver(ITranslationService translations)
{
    /// <param name="count">Quantidade do item: 1 usa o singular; qualquer outro valor, o plural.</param>
    public string ActionLabel(string actionKey, decimal count, string locale) =>
        translations.TranslateAction(actionKey, plural: count != 1m, locale);

    public string PositionLabel(string positionCode, string locale) =>
        translations.TranslatePosition(positionCode, locale);
}
