import type { Metadata } from "next";
import { hasLocale } from "next-intl";
import { getTranslations, setRequestLocale } from "next-intl/server";
import { notFound } from "next/navigation";

import { LeaderboardTable } from "@/components/ranking/LeaderboardTable";
import { PositionFilter } from "@/components/ranking/PositionFilter";
import { RankingMessage } from "@/components/ranking/RankingMessage";
import { RankingPagination } from "@/components/ranking/RankingPagination";
import {
  DEFAULT_SEASON,
  parseRankingQuery,
  RANKING_ELIGIBILITY,
  RANKING_PAGE_SIZE,
  type RankingQuery,
} from "@/components/ranking/rankingQuery";
import { SearchBar } from "@/components/ranking/SearchBar";
import { routing, type Locale } from "@/i18n/routing";
import { apiGet, type ApiError } from "@/lib/api";
import { formatSeason } from "@/lib/format";
import { buildAlternates } from "@/lib/seo";
import type { ApiResponse, SeasonRankingItem } from "@/lib/types";

export async function generateMetadata({ params }: PageProps<"/[locale]/ranking">): Promise<Metadata> {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) return {};

  const t = await getTranslations({ locale, namespace: "ranking.metadata" });
  const season = formatSeason(DEFAULT_SEASON);

  return {
    title: t("title", { season }),
    description: t("description", { season }),
    alternates: buildAlternates(locale, "/ranking"),
  };
}

async function loadRanking(locale: Locale, query: RankingQuery): Promise<ApiResponse<SeasonRankingItem[]> | null> {
  try {
    return await apiGet<SeasonRankingItem[]>("/ranking", {
      locale,
      query: {
        seasonYear: query.season,
        position: query.position,
        search: query.search,
        page: query.page,
        pageSize: RANKING_PAGE_SIZE,
      },
    });
  } catch (error) {
    console.error("Failed to load ranking", error as ApiError);
    return null;
  }
}

export default async function RankingPage({ params, searchParams }: PageProps<"/[locale]/ranking">) {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) {
    notFound();
  }

  setRequestLocale(locale);

  const query = parseRankingQuery(await searchParams);
  const [t, response] = await Promise.all([getTranslations("ranking"), loadRanking(locale, query)]);
  const season = formatSeason(query.season);
  const items = response?.data ?? [];

  return (
    <section className="mx-auto flex max-w-6xl flex-col gap-6 px-4 py-12 sm:px-6">
      <header className="flex flex-col gap-2">
        <h1 className="text-3xl font-black sm:text-4xl">{t("title")}</h1>
        <p className="text-secondary">{t("subtitle", { season })}</p>
      </header>

      <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
        <PositionFilter query={query} />
        <SearchBar query={query} />
      </div>

      {response === null ? (
        <RankingMessage tone="danger" title={t("error.title")} description={t("error.description")} />
      ) : items.length === 0 ? (
        <RankingMessage title={query.search ? t("emptySearch", { search: query.search }) : t("empty")} />
      ) : (
        <LeaderboardTable items={items} caption={t("caption", { season })} byPosition={query.position !== undefined} />
      )}

      <RankingPagination query={query} totalPages={response?.meta?.totalPages ?? 0} />

      <footer className="flex flex-col gap-1 text-sm text-muted">
        <p>{t("notes.eligibility", RANKING_ELIGIBILITY)}</p>
        <p>{t("notes.sample")}</p>
      </footer>
    </section>
  );
}
