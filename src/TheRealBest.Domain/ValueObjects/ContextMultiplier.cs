namespace TheRealBest.Domain.ValueObjects;

/// <summary>
/// Multiplicadores contextuais da partida: torneio, qualidade do adversÃ¡rio e fator clutch.
/// </summary>
public sealed record ContextMultiplier(
    decimal TournamentMultiplier,
    decimal OpponentMultiplier,
    decimal ClutchMultiplier
)
{
    public decimal Combined => Math.Round(TournamentMultiplier * OpponentMultiplier * ClutchMultiplier, 4);

    public static ContextMultiplier Default => new(1.0m, 1.0m, 1.0m);
}