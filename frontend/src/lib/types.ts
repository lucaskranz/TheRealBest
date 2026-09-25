// Tipos que espelham os DTOs da API .NET (JSON em camelCase).

export const playerPositions = ["GK", "CB", "FB", "CDM", "CM", "CAM", "W", "ST"] as const;

export type PlayerPosition = (typeof playerPositions)[number];

export function isPlayerPosition(value: unknown): value is PlayerPosition {
  return typeof value === "string" && (playerPositions as readonly string[]).includes(value);
}

export interface ApiMeta {
  page?: number;
  pageSize?: number;
  totalCount?: number;
  totalPages?: number;
  hasNextPage?: boolean;
  hasPreviousPage?: boolean;
  timestamp: string;
  version: string;
}

export interface ApiResponse<T> {
  data: T | null;
  meta: ApiMeta | null;
  errors: string[] | null;
}

export interface TopMatchItem {
  matchId: string;
  date: string;
  phase: string;
  mps: number;
}

export interface SeasonRankingItem {
  playerId: string;
  playerName: string;
  nationality: string;
  photoUrl: string | null;
  primaryPosition: PlayerPosition;
  primaryPositionLabel: string;
  teamName: string | null;
  teamLogoUrl: string | null;
  overallRank: number;
  positionRank: number;
  fssScore: number;
  mpsAverage: number;
  presenceFactor: number;
  clutchIndex: number;
  totalMatches: number;
  totalMinutes: number;
  isRankingEligible: boolean;
  recalculatedAt: string;
  topMatches: TopMatchItem[];
}

export const attributeCategories = [
  "Finishing",
  "Creation",
  "Possession",
  "Defending",
  "Duels",
  "Goalkeeping",
  "Discipline",
] as const;

export type AttributeCategory = (typeof attributeCategories)[number];

export interface PlayerAttribute {
  category: AttributeCategory;
  per90: number;
  /** Percentil entre jogadores da mesma posição; nulo quando há poucos para comparar. */
  percentile: number | null;
  peerCount: number;
}

export interface PlayerMatchStat {
  matchId: string;
  matchDate: string;
  competitionName: string;
  homeTeamName: string;
  awayTeamName: string;
  homeScore: number | null;
  awayScore: number | null;
  teamName: string;
  positionPlayed: PlayerPosition;
  positionPlayedLabel: string;
  minutesPlayed: number;
  goals: number;
  assists: number;
  yellowCards: number;
  redCards: number;
  finalMps: number | null;
  countsTowardsSeason: boolean;
}

export interface PlayerProfile {
  id: string;
  name: string;
  nationality: string;
  primaryPosition: PlayerPosition;
  primaryPositionLabel: string;
  photoUrl: string | null;
  dateOfBirth: string | null;
  seasonRanking: SeasonRankingItem | null;
  recentMatches: PlayerMatchStat[];
  attributes: PlayerAttribute[];
}

export interface AuditActionItem {
  actionKey: string;
  label: string;
  count: number;
  unitWeight: number;
  totalPoints: number;
  minute: number | null;
  minutesFactor: number;
  category: AttributeCategory | "Unknown";
}

export interface AuditFormula {
  baseScore: number;
  subtotalRaw: number;
  positionBaseline: number;
  minutesFactor: number;
  tournamentMultiplier: number;
  opponentMultiplier: number;
  clutchMultiplier: number;
  contextMultiplierCombined: number;
  finalMps: number;
  countsTowardsSeason: boolean;
}

export interface MatchReceipt {
  scoreId: string;
  matchId: string;
  playerId: string;
  playerName: string;
  playerPosition: PlayerPosition;
  playerPositionLabel: string;
  photoUrl: string | null;
  matchDate: string;
  competitionName: string;
  homeTeamName: string;
  awayTeamName: string;
  homeScore: number | null;
  awayScore: number | null;
  minutesPlayed: number;
  algorithmVersion: number;
  calculatedAt: string;
  formula: AuditFormula;
  positiveActions: AuditActionItem[];
  penalties: AuditActionItem[];
}

/** Espelha API/Formula/FormulaDescriptor.cs: o algoritmo em vigor, gerado das constantes do motor. */
export interface FormulaPosition {
  code: PlayerPosition;
  label: string;
  baseline: number;
  /** Posição cuja matriz de pesos é reutilizada (CM usa a de CDM). */
  sharesMatrixWith: PlayerPosition | null;
}

export type ActionCategory = "Finishing" | "Creation" | "Possession" | "Defending" | "Duels" | "Goalkeeping" | "Discipline";

export interface FormulaAction {
  key: string;
  label: string;
  category: ActionCategory;
  isPenalty: boolean;
  scalesWithMinutes: boolean;
  isDecisive: boolean;
  /** Falso quando a fonte atual não fornece a estatística: o peso existe, mas hoje soma zero. */
  hasDataSource: boolean;
  weights: Partial<Record<PlayerPosition, number>>;
}

export type TournamentWeightKey =
  | "worldCup"
  | "uclKnockout"
  | "continentalNationalTeams"
  | "uclLeaguePhase"
  | "topLeague"
  | "domesticCupLateStage"
  | "other";
export type OpponentWeightKey = "top10" | "top30" | "midTable" | "weak" | "unrated";
export type ClutchWeightKey = "tight" | "open" | "junkTime";

export interface Formula {
  algorithmVersion: number;
  mps: { baseScore: number; minScore: number; maxScore: number };
  minutes: { fullFactorFromMinute: number; regulationMinutes: number; extraTimeDivisor: number; minMinutesForSeason: number };
  positions: FormulaPosition[];
  actions: FormulaAction[];
  tournamentWeights: { key: TournamentWeightKey; value: number }[];
  opponentWeights: { key: OpponentWeightKey; minElo: number | null; value: number }[];
  clutchWeights: { key: ClutchWeightKey; minMargin: number; maxMargin: number | null; value: number }[];
  season: { fullPresenceMinutes: number; minMatchesForRanking: number; minMinutesForRanking: number };
}

export type ComparisonStatus = "ranked" | "notEligible" | "noData";

export interface BallonDorEntry {
  officialRank: number;
  name: string;
  club: string;
  status: ComparisonStatus;
  playerId: string | null;
  photoUrl: string | null;
  primaryPositionLabel: string | null;
  ourRank: number | null;
  /** Colocação oficial menos a nossa: positivo = o índice coloca o jogador mais alto que o júri. */
  rankDelta: number | null;
  fssScore: number | null;
  totalMatches: number | null;
  totalMinutes: number | null;
}

export interface UnnominatedPlayer {
  playerId: string;
  name: string;
  photoUrl: string | null;
  primaryPositionLabel: string;
  ourRank: number;
  fssScore: number;
}

export interface BallonDorComparison {
  year: number;
  seasonYear: number;
  sourceName: string;
  sourceUrl: string;
  eligiblePlayers: number;
  entries: BallonDorEntry[];
  unnominated: UnnominatedPlayer[];
}

const guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export function isGuid(value: string): boolean {
  return guidPattern.test(value);
}
