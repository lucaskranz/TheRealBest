import type { Metadata } from "next";
import { hasLocale } from "next-intl";
import { getFormatter, getTranslations, setRequestLocale } from "next-intl/server";
import { notFound } from "next/navigation";

import { ComparisonTable } from "@/components/ballon/ComparisonTable";
import { PlayerAvatar } from "@/components/ranking/PlayerAvatar";
import { RankingMessage } from "@/components/ranking/RankingMessage";
import { Card } from "@/components/ui/Card";
import { Link } from "@/i18n/navigation";
import { routing, type Locale } from "@/i18n/routing";
import { apiGet } from "@/lib/api";
import { formatSeason } from "@/lib/format";
import { buildAlternates } from "@/lib/seo";
import type { BallonDorComparison, ComparisonStatus, Formula } from "@/lib/types";

/** Edição exibida: a mais recente com classificação oficial cadastrada no backend. */
const BALLON_YEAR = 2024;
const BALLON_SEASON = BALLON_YEAR - 1;

export async function generateMetadata({ params }: PageProps<"/[locale]/vs-ballon">): Promise<Metadata> {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) return {};

  const t = await getTranslations({ locale, namespace: "ballon.metadata" });
  const values = { year: BALLON_YEAR, season: formatSeason(BALLON_SEASON) };

  return {
    title: t("title", values),
    description: t("description", values),
    alternates: buildAlternates(locale, "/vs-ballon"),
  };
}

async function load(locale: Locale): Promise<{ comparison: BallonDorComparison; formula: Formula } | null> {
  try {
    const [comparison, formula] = await Promise.all([
      apiGet<BallonDorComparison>(`/comparison/ballon-dor/${BALLON_YEAR}`, { locale }),
      apiGet<Formula>("/formula", { locale }),
    ]);
    return comparison.data && formula.data ? { comparison: comparison.data, formula: formula.data } : null;
  } catch (error) {
    console.error("Failed to load Ballon d'Or comparison", error);
    return null;
  }
}

export default async function VsBallonPage({ params }: PageProps<"/[locale]/vs-ballon">) {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) notFound();

  setRequestLocale(locale);

  const [t, format, data] = await Promise.all([getTranslations("ballon"), getFormatter(), load(locale)]);
  const season = formatSeason(data?.comparison.seasonYear ?? BALLON_SEASON);
  const year = data?.comparison.year ?? BALLON_YEAR;

  return (
    <section className="mx-auto flex max-w-6xl flex-col gap-8 px-4 py-12 sm:px-6">
      <header className="flex flex-col gap-2">
        <h1 className="text-3xl font-black sm:text-4xl">{t("title", { year })}</h1>
        <p className="max-w-3xl text-secondary">{t("subtitle", { season })}</p>
      </header>

      {data === null ? (
        <RankingMessage tone="danger" title={t("error.title")} description={t("error.description")} />
      ) : (
        <>
          <Summary entries={data.comparison.entries} />

          <p className="max-w-3xl text-sm text-secondary">
            {t("sampleNote", {
              eligible: data.comparison.eligiblePlayers,
              matches: data.formula.season.minMatchesForRanking,
              minutes: data.formula.season.minMinutesForRanking,
            })}
          </p>

          <div className="flex flex-col gap-2">
            <ComparisonTable entries={data.comparison.entries} caption={t("table.caption", { year, season })} />
            <p className="text-xs text-muted">{t("legend.delta")}</p>
          </div>

          {data.comparison.unnominated.length > 0 && (
            <section aria-labelledby="unnominated-title" className="flex flex-col gap-4">
              <header className="flex flex-col gap-1">
                <h2 id="unnominated-title" className="text-2xl font-black">
                  {t("unnominated.title")}
                </h2>
                <p className="text-secondary">{t("unnominated.description")}</p>
              </header>
              <ul className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                {data.comparison.unnominated.map((player) => (
                  <li key={player.playerId}>
                    <Link href={`/player/${player.playerId}`} className="group block">
                      <Card interactive className="flex items-center gap-4 p-4">
                        <PlayerAvatar name={player.name} photoUrl={player.photoUrl} size={48} />
                        <span className="flex min-w-0 flex-1 flex-col">
                          <span className="truncate font-semibold text-main group-hover:text-gold-glow">{player.name}</span>
                          <span className="truncate text-xs text-muted">
                            {player.primaryPositionLabel} · {t("unnominated.rank", { rank: player.ourRank })}
                          </span>
                        </span>
                        <span className="font-display text-xl font-black text-main tabular-nums">
                          {format.number(player.fssScore, { minimumFractionDigits: 1, maximumFractionDigits: 1 })}
                        </span>
                      </Card>
                    </Link>
                  </li>
                ))}
              </ul>
            </section>
          )}

          <p className="text-xs text-muted">
            {t.rich("source", {
              name: data.comparison.sourceName,
              link: (chunks) => (
                <a
                  href={data.comparison.sourceUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="underline underline-offset-2 hover:text-gold-glow"
                >
                  {chunks}
                </a>
              ),
            })}
          </p>
        </>
      )}
    </section>
  );
}

async function Summary({ entries }: { entries: BallonDorComparison["entries"] }) {
  const t = await getTranslations("ballon.summary");
  const statuses: ComparisonStatus[] = ["ranked", "notEligible", "noData"];

  // dt antes de dd no HTML; o número aparece em cima via flex-col-reverse

  return (
    <dl className="grid grid-cols-3 gap-3">
      {statuses.map((status) => (
        <Card key={status} className="flex flex-col-reverse justify-end gap-1 p-4">
          <dt className="text-sm text-secondary">
            {t(status)} <span className="text-muted">{t("of", { total: entries.length })}</span>
          </dt>
          <dd className="font-display text-3xl font-black text-main tabular-nums">
            {entries.filter((e) => e.status === status).length}
          </dd>
        </Card>
      ))}
    </dl>
  );
}
