namespace TheRealBest.Domain.Interfaces;

/// <summary>
/// Rating Elo de clubes numa data (escala ClubElo). Usado no multiplicador de força do adversário.
/// </summary>
public interface IClubEloProvider
{
    /// <summary>
    /// Rating do clube na data, ou nulo se a fonte estiver indisponível ou o clube não for encontrado.
    /// Nunca lança por falha da fonte: a ausência de rating resulta em multiplicador neutro.
    /// </summary>
    Task<int?> GetEloAsync(string teamName, DateOnly date, CancellationToken cancellationToken = default);
}
