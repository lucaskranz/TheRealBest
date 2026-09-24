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
