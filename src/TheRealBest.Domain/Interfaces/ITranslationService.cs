namespace TheRealBest.Domain.Interfaces;

/// <summary>
/// Textos estáticos localizados (rótulos de ações, posições e mensagens). Locales: pt-BR (padrão), en, es.
/// Chaves sem tradução devolvem a própria chave, para nunca quebrar a resposta.
/// </summary>
public interface ITranslationService
{
    /// <param name="actionKey">Chave da ação no recibo (ActionScoreItem.ActionKey, ex.: "goal").</param>
    /// <param name="plural">Forma plural, para quantidades diferentes de 1.</param>
    string TranslateAction(string actionKey, bool plural, string locale);

    /// <param name="positionKey">Código da posição (PlayerPosition, ex.: "CDM").</param>
    string TranslatePosition(string positionKey, string locale);

    string TranslateMessage(string messageKey, string locale, params object[] args);
}
