namespace TheRealBest.Scoring.Rules;

using System.Text;
using TheRealBest.Domain.Enums;

/// <summary>
/// Metadados de uma ação pontuável.
/// </summary>
/// <param name="ScalesWithMinutes">
/// Ações de volume (desarmes, passes, duelos...) são proporcionalizadas pelo FatorMinutos.
/// Eventos pontuais (gols, cartões, erros...) valem integralmente: um gol de quem entrou aos 80' é um gol inteiro.
/// </param>
/// <param name="IsDecisive">
/// Ação com impacto direto no placar. Jogadores com menos de 20 minutos só entram no FSS se tiverem uma.
/// </param>
public sealed record ActionDefinition(
    ActionType Type,
    string Key,
    bool IsPenalty,
    bool ScalesWithMinutes,
    bool IsDecisive,
    string Description);

/// <summary>
/// Catálogo das ações usadas pelo Fair Player Index, na ordem em que aparecem no recibo.
/// </summary>
public static class ActionCatalog
{
    private static readonly ActionDefinition[] Definitions =
    [
        // Ações positivas
        Positive(ActionType.Goal, "Gol (bola rolando)", scales: false, decisive: true),
        Positive(ActionType.PenaltyGoal, "Gol (pênalti)", scales: false, decisive: true),
        Positive(ActionType.Assist, "Assistência direta", scales: false, decisive: true),
        Positive(ActionType.ExpectedAssists, "Assistência esperada (xA, por unidade)"),
        Positive(ActionType.BigChanceCreated, "Grande chance criada"),
        Positive(ActionType.ShotOnTarget, "Finalização no alvo"),
        Positive(ActionType.ExpectedGoalsOverperformance, "xG superado (gols - xG)", scales: false),
        Positive(ActionType.KeyPass, "Passe decisivo"),
        Positive(ActionType.ProgressivePass, "Passe progressivo"),
        Positive(ActionType.PassAccuracyBonus, "Precisão de passe acima de 85%"),
        Positive(ActionType.DribbleSuccess, "Drible bem-sucedido"),
        Positive(ActionType.FoulDrawn, "Falta sofrida"),
        Positive(ActionType.Tackle, "Desarme"),
        Positive(ActionType.Interception, "Interceptação"),
        Positive(ActionType.AerialDuelWon, "Duelo aéreo ganho"),
        Positive(ActionType.DuelWon, "Duelo no chão ganho"),
        Positive(ActionType.BallRecovery, "Recuperação de posse"),
        Positive(ActionType.CleanSheet, "Clean sheet (mais de 60 min)", scales: false),
        Positive(ActionType.Save, "Defesa"),
        Positive(ActionType.GoalsPrevented, "Gols prevenidos (xGOT - gols sofridos)"),
        Positive(ActionType.PenaltySaved, "Pênalti defendido", scales: false, decisive: true),
        Positive(ActionType.HighClaim, "Saída aérea com sucesso"),

        // Penalidades
        Penalty(ActionType.ErrorLeadingToGoal, "Erro grave que levou a gol", decisive: true),
        Penalty(ActionType.ErrorLeadingToShot, "Erro que levou a finalização rival"),
        Penalty(ActionType.PenaltyCommitted, "Pênalti cometido", decisive: true),
        Penalty(ActionType.OwnGoal, "Gol contra", decisive: true),
        Penalty(ActionType.YellowCard, "Cartão amarelo"),
        Penalty(ActionType.RedCard, "Cartão vermelho", decisive: true),
        Penalty(ActionType.BigChanceMissed, "Grande chance perdida"),
        Penalty(ActionType.Turnover, "Perda de posse no campo defensivo", scales: true),
        Penalty(ActionType.Offside, "Impedimento", scales: true),
        Penalty(ActionType.GoalConceded, "Gol sofrido pelo time (após o 1º)"),
    ];

    private static readonly Dictionary<ActionType, ActionDefinition> ByType = Definitions.ToDictionary(d => d.Type);

    public static IReadOnlyList<ActionDefinition> All => Definitions;

    public static ActionDefinition Get(ActionType type) =>
        ByType.TryGetValue(type, out var definition)
            ? definition
            : throw new ArgumentOutOfRangeException(nameof(type), type, "Action is not part of the scoring catalog.");

    public static bool Contains(ActionType type) => ByType.ContainsKey(type);

    private static ActionDefinition Positive(ActionType type, string description, bool scales = true, bool decisive = false) =>
        new(type, ToSnakeCase(type.ToString()), IsPenalty: false, scales, decisive, description);

    private static ActionDefinition Penalty(ActionType type, string description, bool scales = false, bool decisive = false) =>
        new(type, ToSnakeCase(type.ToString()), IsPenalty: true, scales, decisive, description);

    private static string ToSnakeCase(string pascalCase)
    {
        var builder = new StringBuilder(pascalCase.Length + 8);
        for (var i = 0; i < pascalCase.Length; i++)
        {
            var c = pascalCase[i];
            if (char.IsUpper(c) && i > 0)
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(c));
        }

        return builder.ToString();
    }
}
