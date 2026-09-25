import type { Metadata } from "next";
import { hasLocale } from "next-intl";
import { getTranslations, setRequestLocale } from "next-intl/server";
import { notFound } from "next/navigation";
import { cache } from "react";

import { AttributeBars } from "@/components/player/AttributeBars";
import { MatchList } from "@/components/player/MatchList";
import { PlayerHero } from "@/components/player/PlayerHero";
import { SeasonSummary } from "@/components/player/SeasonSummary";
import { DEFAULT_SEASON } from "@/components/ranking/rankingQuery";
import { RankingMessage } from "@/components/ranking/RankingMessage";
import { routing, type Locale } from "@/i18n/routing";
import { ApiError, apiGet } from "@/lib/api";
import { formatSeason } from "@/lib/format";
import { buildAlternates } from "@/lib/seo";
import { isGuid, type PlayerProfile } from "@/lib/types";

/** Espelha PlayerAttributeCalculator.MinMinutesForComparison no backend. */
const ATTRIBUTE_MIN_MINUTES = 270;

type LoadResult = { profile: PlayerProfile } | { notFound: true } | { failed: true };

// cache(): generateMetadata e a página compartilham a mesma chamada à API na requisição
const loadProfile = cache(async (locale: Locale, id: string): Promise<LoadResult> => {
  if (!isGuid(id)) return { notFound: true };

  try {
    const response = await apiGet<PlayerProfile>(`/players/${id}`, { locale, query: { seasonYear: DEFAULT_SEASON } });
    return response.data ? { profile: response.data } : { notFound: true };
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return { notFound: true };
    console.error("Failed to load player profile", error);
    return { failed: true };
  }
});

export async function generateMetadata({ params }: PageProps<"/[locale]/player/[id]">): Promise<Metadata> {
  const { locale, id } = await params;
  if (!hasLocale(routing.locales, locale)) return {};

  const result = await loadProfile(locale, id);
  if (!("profile" in result)) return {};

  const t = await getTranslations({ locale, namespace: "player.metadata" });
  const name = result.profile.name;
  const season = formatSeason(DEFAULT_SEASON);

  return {
    title: t("title", { name, season }),
    description: t("description", { name }),
    alternates: buildAlternates(locale, `/player/${id}`),
    openGraph: result.profile.photoUrl ? { images: [result.profile.photoUrl] } : undefined,
  };
}

export default async function PlayerPage({ params }: PageProps<"/[locale]/player/[id]">) {
  const { locale, id } = await params;
  if (!hasLocale(routing.locales, locale)) notFound();

  setRequestLocale(locale);

  const result = await loadProfile(locale, id);
  if ("notFound" in result) notFound();

  if ("failed" in result) {
    const t = await getTranslations("player.error");
    return (
      <section className="mx-auto max-w-6xl px-4 py-12 sm:px-6">
        <RankingMessage tone="danger" title={t("title")} description={t("description")} />
      </section>
    );
  }

  const { profile } = result;

  return (
    <section className="mx-auto flex max-w-6xl flex-col gap-8 px-4 py-12 sm:px-6">
      <PlayerHero profile={profile} />
      <div className="grid gap-6 lg:grid-cols-2">
        <SeasonSummary ranking={profile.seasonRanking} position={profile.primaryPosition} season={formatSeason(DEFAULT_SEASON)} />
        <AttributeBars attributes={profile.attributes} positionLabel={profile.primaryPositionLabel} minMinutes={ATTRIBUTE_MIN_MINUTES} />
      </div>
      <MatchList playerId={profile.id} matches={profile.recentMatches} />
    </section>
  );
}
