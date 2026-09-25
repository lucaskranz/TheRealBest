import type { Metadata } from "next";
import { hasLocale } from "next-intl";
import { getTranslations, setRequestLocale } from "next-intl/server";
import { notFound } from "next/navigation";
import { cache } from "react";

import { RankingMessage } from "@/components/ranking/RankingMessage";
import { MatchReceipt } from "@/components/receipt/MatchReceipt";
import { routing, type Locale } from "@/i18n/routing";
import { ApiError, apiGet } from "@/lib/api";
import { matchTitle } from "@/lib/format";
import { buildAlternates } from "@/lib/seo";
import { isGuid, type MatchReceipt as MatchReceiptData } from "@/lib/types";

type LoadResult = { receipt: MatchReceiptData } | { notFound: true } | { failed: true };

const loadReceipt = cache(async (locale: Locale, playerId: string, matchId: string): Promise<LoadResult> => {
  if (!isGuid(playerId) || !isGuid(matchId)) return { notFound: true };

  try {
    const response = await apiGet<MatchReceiptData>(`/audit/matches/${matchId}/players/${playerId}`, { locale });
    return response.data ? { receipt: response.data } : { notFound: true };
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return { notFound: true };
    console.error("Failed to load match receipt", error);
    return { failed: true };
  }
});

export async function generateMetadata({ params }: PageProps<"/[locale]/player/[id]/match/[matchId]">): Promise<Metadata> {
  const { locale, id, matchId } = await params;
  if (!hasLocale(routing.locales, locale)) return {};

  const result = await loadReceipt(locale, id, matchId);
  if (!("receipt" in result)) return {};

  const { receipt } = result;
  const t = await getTranslations({ locale, namespace: "receipt.metadata" });
  const match = matchTitle(receipt.homeTeamName, receipt.awayTeamName, receipt.homeScore, receipt.awayScore);

  return {
    title: t("title", { player: receipt.playerName, match }),
    description: t("description", { player: receipt.playerName, match }),
    alternates: buildAlternates(locale, `/player/${id}/match/${matchId}`),
  };
}

export default async function MatchReceiptPage({ params }: PageProps<"/[locale]/player/[id]/match/[matchId]">) {
  const { locale, id, matchId } = await params;
  if (!hasLocale(routing.locales, locale)) notFound();

  setRequestLocale(locale);

  const result = await loadReceipt(locale, id, matchId);
  if ("notFound" in result) notFound();

  if ("failed" in result) {
    const t = await getTranslations("receipt.error");
    return (
      <section className="mx-auto max-w-6xl px-4 py-12 sm:px-6">
        <RankingMessage tone="danger" title={t("title")} description={t("description")} />
      </section>
    );
  }

  return (
    <section className="mx-auto max-w-6xl px-4 py-12 sm:px-6">
      <MatchReceipt receipt={result.receipt} />
    </section>
  );
}
