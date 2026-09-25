namespace TheRealBest.Domain.Enums;

public static class CompetitionTierExtensions
{
    /// <summary>Competição de clubes (tem Elo no ClubElo); seleções usam multiplicador de adversário neutro.</summary>
    public static bool IsClubCompetition(this CompetitionTier tier) =>
        tier is not (CompetitionTier.WorldCup or CompetitionTier.InternationalContinental);
}
