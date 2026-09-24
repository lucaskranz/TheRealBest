namespace TheRealBest.Domain.Interfaces;

public interface ITranslationService
{
    string TranslateAction(string actionKey, string locale);
    string TranslatePosition(string positionKey, string locale);
    string TranslateMessage(string messageKey, string locale, params object[] args);
}