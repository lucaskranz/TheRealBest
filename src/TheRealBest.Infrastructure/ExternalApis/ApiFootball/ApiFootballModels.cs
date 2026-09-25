namespace TheRealBest.Infrastructure.ExternalApis.ApiFootball;

using System.Text.Json;
using System.Text.Json.Serialization;

// Modelos das respostas da API-Football v3. Campos numéricos vêm como null quando o evento não ocorreu.

/// <summary>
/// Envelope comum. Erros de negócio (plano, cota, chave) chegam com HTTP 200 em "errors",
/// que é um array vazio quando não há erro e um objeto { chave: mensagem } quando há.
/// </summary>
public sealed record ApiFootballResponse<T>(JsonElement Errors, int Results, IReadOnlyList<T> Response);

public sealed record TeamInfo(int Id, string Name, string? Logo);

// /fixtures

public sealed record FixtureItem(FixtureInfo Fixture, LeagueInfo League, TeamsInfo Teams, GoalsInfo Goals);

public sealed record FixtureInfo(int Id, DateTimeOffset Date, FixtureStatus Status);

public sealed record FixtureStatus(string Short, int? Elapsed);

public sealed record LeagueInfo(int Id, string Name, string? Country, int Season, string Round);

public sealed record TeamsInfo(TeamInfo Home, TeamInfo Away);

public sealed record GoalsInfo(int? Home, int? Away);

// /fixtures?ids=... (detalhe de várias partidas)

/// <summary>
/// Partida com os detalhes embutidos: os mesmos formatos de /fixtures/players, /fixtures/events e /fixtures/lineups.
/// Listas ausentes (partida sem cobertura de jogadores) chegam nulas ou vazias.
/// </summary>
public sealed record FixtureDetailItem(
    FixtureInfo Fixture,
    LeagueInfo League,
    TeamsInfo Teams,
    GoalsInfo Goals,
    IReadOnlyList<EventItem>? Events,
    IReadOnlyList<LineupItem>? Lineups,
    IReadOnlyList<FixturePlayersItem>? Players);

// /fixtures/players

public sealed record FixturePlayersItem(TeamInfo Team, IReadOnlyList<PlayerEntry> Players);

public sealed record PlayerEntry(PlayerInfo Player, IReadOnlyList<PlayerStatistics> Statistics);

public sealed record PlayerInfo(int Id, string Name, string? Photo);

public sealed record PlayerStatistics(
    GamesStats Games,
    int? Offsides,
    ShotsStats Shots,
    GoalsStats Goals,
    PassesStats Passes,
    TacklesStats Tackles,
    DuelsStats Duels,
    DribblesStats Dribbles,
    FoulsStats Fouls,
    CardsStats Cards,
    PenaltyStats Penalty);

public sealed record GamesStats(int? Minutes, string? Position, bool Substitute);

public sealed record ShotsStats(int? Total, int? On);

public sealed record GoalsStats(int? Total, int? Conceded, int? Assists, int? Saves);

/// <param name="Accuracy">
/// Apesar do nome, é a QUANTIDADE de passes certos (ex.: "61" de 66), não o percentual. Chega como string.
/// </param>
public sealed record PassesStats(int? Total, int? Key, JsonElement Accuracy);

public sealed record TacklesStats(int? Total, int? Blocks, int? Interceptions);

public sealed record DuelsStats(int? Total, int? Won);

public sealed record DribblesStats(int? Attempts, int? Success, int? Past);

public sealed record FoulsStats(int? Drawn, int? Committed);

public sealed record CardsStats(int? Yellow, int? Red);

public sealed record PenaltyStats(
    int? Won,
    [property: JsonPropertyName("commited")] int? Committed,
    int? Scored,
    int? Missed,
    int? Saved);

// /fixtures/events

/// <param name="Player">Em substituições ("subst"), é o jogador que SAI.</param>
/// <param name="Assist">Em substituições ("subst"), é o jogador que ENTRA.</param>
public sealed record EventItem(
    EventTime Time,
    TeamInfo Team,
    EventPerson Player,
    EventPerson Assist,
    string Type,
    string? Detail,
    string? Comments);

public sealed record EventTime(int Elapsed, int? Extra);

public sealed record EventPerson(int? Id, string? Name);

// /fixtures/lineups

/// <param name="StartXI">Nulo quando a fonte não tem a escalação (ex.: fases preliminares de copas).</param>
public sealed record LineupItem(
    TeamInfo Team,
    string? Formation,
    IReadOnlyList<LineupSlot>? StartXI,
    IReadOnlyList<LineupSlot>? Substitutes);

public sealed record LineupSlot(LineupPlayer Player);

/// <param name="Grid">"linha:coluna" na formação (linha 1 = goleiro). Nulo para reservas.</param>
public sealed record LineupPlayer(int Id, string Name, string? Pos, string? Grid);
