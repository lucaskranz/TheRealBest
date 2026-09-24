import { isPlayerPosition, type PlayerPosition } from "@/lib/types";

/** Espelha SeasonScoreCalculator.MinMatchesForRanking / MinMinutesForRanking no backend. */
export const RANKING_ELIGIBILITY = { matches: 10, minutes: 900 } as const;

export const DEFAULT_SEASON = 2023;
export const RANKING_PAGE_SIZE = 25;

export interface RankingQuery {
  season: number;
  position?: PlayerPosition;
  search?: string;
  page: number;
}

type SearchParams = Record<string, string | string[] | undefined>;

function first(value: string | string[] | undefined): string | undefined {
  return Array.isArray(value) ? value[0] : value;
}

/** Lê os filtros da URL, descartando valores inválidos em vez de falhar. */
export function parseRankingQuery(params: SearchParams): RankingQuery {
  const position = first(params.position);
  const page = Number.parseInt(first(params.page) ?? "", 10);
  const season = Number.parseInt(first(params.season) ?? "", 10);
  const search = first(params.search)?.trim();

  return {
    season: Number.isFinite(season) ? season : DEFAULT_SEASON,
    position: isPlayerPosition(position) ? position : undefined,
    search: search ? search.slice(0, 60) : undefined,
    page: Number.isFinite(page) && page > 0 ? page : 1,
  };
}

/** href do ranking com os filtros; omite valores padrão para manter a URL limpa. */
export function rankingHref(query: RankingQuery, changes: Partial<RankingQuery> = {}) {
  const next = { ...query, ...changes };
  const params: Record<string, string> = {};

  if (next.season !== DEFAULT_SEASON) params.season = String(next.season);
  if (next.position) params.position = next.position;
  if (next.search) params.search = next.search;
  if (next.page > 1) params.page = String(next.page);

  return { pathname: "/ranking" as const, query: params };
}
