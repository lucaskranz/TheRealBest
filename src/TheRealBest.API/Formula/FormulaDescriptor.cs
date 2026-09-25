namespace TheRealBest.API.Formula;

/// <summary>
/// Descrição pública do algoritmo em vigor, gerada a partir das constantes do motor de pontuação.
/// A página "O Algoritmo" só exibe o que vem daqui, para nunca divergir do cálculo real.
/// </summary>
public sealed record FormulaDescriptor(
    int AlgorithmVersion,
    MpsRules Mps,
    MinutesRules Minutes,
    IReadOnlyList<FormulaPosition> Positions,
    IReadOnlyList<FormulaAction> Actions,
    IReadOnlyList<MultiplierStep> TournamentWeights,
    IReadOnlyList<OpponentStep> OpponentWeights,
    IReadOnlyList<ClutchStep> ClutchWeights,
    SeasonRules Season);

public sealed record MpsRules(decimal BaseScore, decimal MinScore, decimal MaxScore);

/// <param name="ExtraTimeDivisor">Na prorrogação, FatorMinutos = 1 + (M − Regulation) / ExtraTimeDivisor.</param>
/// <param name="MinMinutesForSeason">Abaixo disso a partida só conta no FSS se o jogador tiver uma ação decisiva.</param>
public sealed record MinutesRules(int FullFactorFromMinute, int RegulationMinutes, int ExtraTimeDivisor, int MinMinutesForSeason);

/// <param name="SharesMatrixWith">Posição cuja matriz de pesos é reutilizada (CM usa a de CDM).</param>
public sealed record FormulaPosition(string Code, string Label, decimal Baseline, string? SharesMatrixWith);

/// <param name="HasDataSource">Falso quando a fonte atual não fornece a estatística: o peso existe, mas hoje soma zero.</param>
/// <param name="Weights">Peso por código de posição; ausente = a ação não pontua naquela posição.</param>
public sealed record FormulaAction(
    string Key,
    string Label,
    string Category,
    bool IsPenalty,
    bool ScalesWithMinutes,
    bool IsDecisive,
    bool HasDataSource,
    IReadOnlyDictionary<string, decimal> Weights);

/// <param name="Key">Identificador estável, traduzido no frontend (ex.: "worldCup").</param>
public sealed record MultiplierStep(string Key, decimal Value);

/// <param name="MinElo">Limite inferior da faixa; nulo = sem rating (seleções ou fonte indisponível).</param>
public sealed record OpponentStep(string Key, int? MinElo, decimal Value);

public sealed record ClutchStep(string Key, int MinMargin, int? MaxMargin, decimal Value);

public sealed record SeasonRules(int FullPresenceMinutes, int MinMatchesForRanking, int MinMinutesForRanking);
