import { useFormatter, useTranslations } from "next-intl";

import { RANKING_ELIGIBILITY } from "@/components/ranking/rankingQuery";
import { Card } from "@/components/ui/Card";
import type { PlayerPosition, SeasonRankingItem } from "@/lib/types";

interface SeasonSummaryProps {
  ranking: SeasonRankingItem | null;
  position: PlayerPosition;
  season: string;
}

/** FSS como número principal; posições no ranking e volume de jogo como números de apoio. */
export function SeasonSummary({ ranking, position, season }: SeasonSummaryProps) {
  const t = useTranslations("player.summary");
  const positions = useTranslations("positions");
  const format = useFormatter();

  if (!ranking) {
    return <Card className="text-secondary">{t("noRanking")}</Card>;
  }

  const decimal = (value: number, digits = 1) =>
    format.number(value, { minimumFractionDigits: digits, maximumFractionDigits: digits });
  const rank = (value: number) => (ranking.isRankingEligible && value > 0 ? `#${value}` : "—");

  const stats = [
    { label: t("overallRank"), value: rank(ranking.overallRank) },
    { label: t("positionRank", { position: positions(`names.${position}`) }), value: rank(ranking.positionRank) },
    { label: t("mpsAverage"), value: decimal(ranking.mpsAverage) },
    { label: t("matches"), value: format.number(ranking.totalMatches) },
    { label: t("minutes"), value: format.number(ranking.totalMinutes) },
    { label: t("presenceFactor"), value: decimal(ranking.presenceFactor, 2) },
  ];

  return (
    <Card highlighted={ranking.overallRank === 1} className="flex flex-col gap-6">
      <div className="flex flex-col gap-1">
        <p className="text-sm font-semibold tracking-wide text-muted uppercase">{t("title", { season })}</p>
        <p className="flex items-baseline gap-3">
          <span className="font-display text-6xl font-black text-gradient-gold">{decimal(ranking.fssScore)}</span>
          <span className="text-sm font-semibold text-secondary">{t("fss")}</span>
        </p>
        {!ranking.isRankingEligible && (
          <p className="text-sm text-secondary">
            {t("notEligible", {
              matches: ranking.totalMatches,
              minMatches: RANKING_ELIGIBILITY.matches,
              minutes: format.number(ranking.totalMinutes),
              minMinutes: format.number(RANKING_ELIGIBILITY.minutes),
            })}
          </p>
        )}
      </div>

      <dl className="grid grid-cols-2 gap-x-6 gap-y-4 sm:grid-cols-3">
        {stats.map(({ label, value }) => (
          <div key={label} className="flex flex-col gap-0.5">
            <dt className="text-xs text-muted">{label}</dt>
            <dd className="font-display text-2xl font-bold text-main">{value}</dd>
          </div>
        ))}
      </dl>
    </Card>
  );
}
