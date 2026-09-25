namespace TheRealBest.Application.DTOs.Comparison;

public static class ComparisonStatus
{
    /// <summary>Elegível no nosso ranking: tem colocação.</summary>
    public const string Ranked = "ranked";

    /// <summary>Tem partidas importadas, mas abaixo do corte de 10 jogos e 900 minutos.</summary>
    public const string NotEligible = "notEligible";

    /// <summary>Nenhuma partida importada: dados insuficientes.</summary>
    public const string NoData = "noData";
}

/// <param name="RankDelta">Colocação oficial menos a nossa: positivo = o índice coloca o jogador mais alto que o júri.</param>
public sealed record BallonDorEntryDto(
    int OfficialRank,
    string Name,
    string Club,
    string Status,
    Guid? PlayerId,
    string? PhotoUrl,
    string? PrimaryPositionLabel,
    int? OurRank,
    int? RankDelta,
    decimal? FssScore,
    int? TotalMatches,
    int? TotalMinutes);

/// <summary>Jogador bem colocado no índice que não esteve entre os indicados.</summary>
public sealed record UnnominatedPlayerDto(
    Guid PlayerId,
    string Name,
    string? PhotoUrl,
    string PrimaryPositionLabel,
    int OurRank,
    decimal FssScore);

public sealed record BallonDorComparisonDto(
    int Year,
    int SeasonYear,
    string SourceName,
    string SourceUrl,
    int EligiblePlayers,
    IReadOnlyList<BallonDorEntryDto> Entries,
    IReadOnlyList<UnnominatedPlayerDto> Unnominated);
