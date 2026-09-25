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
    ActionCategory Category,
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
        Positive(ActionType.Goal, ActionCategory.Finishing, "Gol (bola rolando)", scales: false, decisive: true),
        Positive(ActionType.PenaltyGoal, ActionCategory.Finishing, "Gol (pênalti)", scales: false, decisive: true),
        Positive(ActionType.Assist, ActionCategory.Creation, "Assistência direta", scales: false, decisive: true),
        Positive(ActionType.ExpectedAssists, ActionCategory.Creation, "Assistência esperada (xA, por unidade)"),
        Positive(ActionType.BigChanceCreated, ActionCategory.Creation, "Grande chance criada"),
        Positive(ActionType.ShotOnTarget, ActionCategory.Finishing, "Finalização no alvo"),
        Positive(ActionType.ExpectedGoalsOverperformance, ActionCategory.Finishing, "xG superado (gols - xG)", scales: false),
        Positive(ActionType.KeyPass, ActionCategory.Creation, "Passe decisivo"),
        Positive(ActionType.ProgressivePass, ActionCategory.Creation, "Passe progressivo"),
        Positive(ActionType.PassAccuracyBonus, ActionCategory.Possession, "Precisão de passe acima de 85%"),
        Positive(ActionType.DribbleSuccess, ActionCategory.Possession, "Drible bem-sucedido"),
        Positive(ActionType.FoulDrawn, ActionCategory.Possession, "Falta sofrida"),
        Positive(ActionType.Tackle, ActionCategory.Defending, "Desarme"),
        Positive(ActionType.Interception, ActionCategory.Defending, "Interceptação"),
        Positive(ActionType.AerialDuelWon, ActionCategory.Duels, "Duelo aéreo ganho"),
        Positive(ActionType.DuelWon, ActionCategory.Duels, "Duelo no chão ganho"),
        Positive(ActionType.BallRecovery, ActionCategory.Defending, "Recuperação de posse"),
        Positive(ActionType.CleanSheet, ActionCategory.Defending, "Clean sheet (mais de 60 min)", scales: false),
        Positive(ActionType.Save, ActionCategory.Goalkeeping, "Defesa"),
        Positive(ActionType.GoalsPrevented, ActionCategory.Goalkeeping, "Gols prevenidos (xGOT - gols sofridos)"),
        Positive(ActionType.PenaltySaved, ActionCategory.Goalkeeping, "Pênalti defendido", scales: false, decisive: true),
        Positive(ActionType.HighClaim, ActionCategory.Goalkeeping, "Saída aérea com sucesso"),

        // Penalidades
        Penalty(ActionType.ErrorLeadingToGoal, ActionCategory.Defending, "Erro grave que levou a gol", decisive: true),
        Penalty(ActionType.ErrorLeadingToShot, ActionCategory.Defending, "Erro que levou a finalização rival"),
        Penalty(ActionType.PenaltyCommitted, ActionCategory.Discipline, "Pênalti cometido", decisive: true),
        Penalty(ActionType.OwnGoal, ActionCategory.Defending, "Gol contra", decisive: true),
        Penalty(ActionType.YellowCard, ActionCategory.Discipline, "Cartão amarelo"),
        Penalty(ActionType.RedCard, ActionCategory.Discipline, "Cartão vermelho", decisive: true),
        Penalty(ActionType.BigChanceMissed, ActionCategory.Finishing, "Grande chance perdida"),
        Penalty(ActionType.Turnover, ActionCategory.Possession, "Perda de posse no campo defensivo", scales: true),
        Penalty(ActionType.Offside, ActionCategory.Finishing, "Impedimento", scales: true),
        Penalty(ActionType.GoalConceded, ActionCategory.Defending, "Gol sofrido pelo time (após o 1º)"),
    ];

    private static readonly Dictionary<ActionType, ActionDefinition> ByType = Definitions.ToDictionary(d => d.Type);

    public static IReadOnlyList<ActionDefinition> All => Definitions;

    public static ActionDefinition Get(ActionType type) =>
        ByType.TryGetValue(type, out var definition)
            ? definition
            : throw new ArgumentOutOfRangeException(nameof(type), type, "Action is not part of the scoring catalog.");

    public static bool Contains(ActionType type) => ByType.ContainsKey(type);

    private static ActionDefinition Positive(ActionType type, ActionCategory category, string description, bool scales = true, bool decisive = false) =>
        new(type, ToSnakeCase(type.ToString()), category, IsPenalty: false, scales, decisive, description);

    private static ActionDefinition Penalty(ActionType type, ActionCategory category, string description, bool scales = false, bool decisive = false) =>
        new(type, ToSnakeCase(type.ToString()), category, IsPenalty: true, scales, decisive, description);

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
