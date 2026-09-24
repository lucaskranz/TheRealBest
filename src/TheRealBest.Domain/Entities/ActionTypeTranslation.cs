namespace TheRealBest.Domain.Entities;

/// <summary>
/// Tradução do label de uma ação do recibo auditável para um locale suportado (pt-BR, en, es).
/// </summary>
public class ActionTypeTranslation : EntityBase
{
    public string ActionKey { get; private set; } = string.Empty;
    public string Locale { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public string? LabelPlural { get; private set; }

    protected ActionTypeTranslation() { }

    public ActionTypeTranslation(
        string actionKey,
        string locale,
        string label,
        string? labelPlural = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        ActionKey = actionKey;
        Locale = locale;
        Label = label;
        LabelPlural = labelPlural;
    }

    public void Update(string label, string? labelPlural)
    {
        Label = label;
        LabelPlural = labelPlural;
        MarkUpdated();
    }
}
