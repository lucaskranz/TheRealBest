namespace TheRealBest.Domain.ValueObjects;

using TheRealBest.Domain.Enums;

/// <summary>
/// Linha estatística bruta de um jogador numa partida, independente da fonte de dados.
/// Métricas que a fonte não fornece ficam em zero.
/// </summary>
public sealed record PlayerStatLine
{
    public required PlayerPosition Position { get; init; }
    public required int MinutesPlayed { get; init; }

    // Ataque e criação
    public int Goals { get; init; }
    public int PenaltiesScored { get; init; }
    public int Assists { get; init; }
    public int ShotsTotal { get; init; }
    public int ShotsOnTarget { get; init; }
    public int KeyPasses { get; init; }
    public int BigChancesCreated { get; init; }
    public int BigChancesMissed { get; init; }
    public decimal Xg { get; init; }
    public decimal Xa { get; init; }
    public int ShotCreatingActions { get; init; }

    // Passe e posse
    public int PassesTotal { get; init; }
    public int PassesAccurate { get; init; }
    public int ProgressivePasses { get; init; }
    public int ProgressiveCarries { get; init; }
    public int Touches { get; init; }
    public int Turnovers { get; init; }

    // Defesa
    public int TacklesTotal { get; init; }
    public int Interceptions { get; init; }
    public int Blocks { get; init; }
    public int BallRecoveries { get; init; }
    public int DuelsTotal { get; init; }
    public int DuelsWon { get; init; }
    public int AerialDuelsTotal { get; init; }
    public int AerialDuelsWon { get; init; }
    public int DribbledPast { get; init; }
    public int ErrorsLeadingToGoal { get; init; }
    public int OwnGoals { get; init; }

    // Goleiro, disciplina e bola parada
    public int Saves { get; init; }
    public int GoalsConceded { get; init; }
    public int PenaltiesSaved { get; init; }
    public bool CleanSheet { get; init; }
    public int FoulsCommitted { get; init; }
    public int FoulsDrawn { get; init; }
    public int YellowCards { get; init; }
    public int RedCards { get; init; }
    public int PenaltiesWon { get; init; }
    public int PenaltiesCommitted { get; init; }
    public int PenaltiesMissed { get; init; }
    public int DribblesAttempted { get; init; }
    public int DribblesSuccess { get; init; }
    public int Offsides { get; init; }
}
