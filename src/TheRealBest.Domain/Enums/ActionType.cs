namespace TheRealBest.Domain.Enums;

/// <summary>
/// AÃ§Ãµes estatÃ­sticas e eventos avaliados na performance do jogador.
/// </summary>
public enum ActionType
{
    // AÃ§Ãµes ofensivas e criaÃ§Ã£o
    Goal = 1,
    Assist = 2,
    KeyPass = 3,
    ProgressivePass = 4,
    ProgressiveCarry = 5,
    BigChanceCreated = 6,
    ShotOnTarget = 7,
    DribbleSuccess = 8,
    FoulDrawn = 9,
    PenaltyWon = 10,

    // AÃ§Ãµes defensivas e goleiro
    Tackle = 11,
    Interception = 12,
    BallRecovery = 13,
    Block = 14,
    DuelWon = 15,
    AerialDuelWon = 16,
    CleanSheet = 17,
    Save = 18,
    PenaltySaved = 19,

    // AÃ§Ãµes negativas e penalidades
    BigChanceMissed = 20,
    Turnover = 21,
    ErrorLeadingToGoal = 22,
    FoulCommitted = 23,
    PenaltyCommitted = 24,
    YellowCard = 25,
    RedCard = 26,
    OwnGoal = 27,
    DribbledPast = 28,
    GoalConceded = 29,
    PenaltyMissed = 30
}