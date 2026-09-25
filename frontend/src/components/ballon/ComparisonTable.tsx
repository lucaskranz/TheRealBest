import { useFormatter, useTranslations } from "next-intl";

import { PlayerAvatar } from "@/components/ranking/PlayerAvatar";
import { Badge } from "@/components/ui/Badge";
import { Link } from "@/i18n/navigation";
import type { BallonDorEntry } from "@/lib/types";
import { cn } from "@/lib/utils";

import { RankDelta } from "./RankDelta";

interface ComparisonTableProps {
  entries: BallonDorEntry[];
  caption: string;
}

/** Os 30 indicados na ordem oficial, com a colocação e a nota no índice ao lado. */
export function ComparisonTable({ entries, caption }: ComparisonTableProps) {
  const t = useTranslations("ballon.table");
  const format = useFormatter();
  const header = "px-3 py-3 text-xs font-semibold tracking-wide text-muted uppercase";

  return (
    <div className="glass overflow-hidden rounded-lg shadow-card">
      <table className="w-full table-fixed border-collapse text-left">
        <caption className="sr-only">{caption}</caption>
        <colgroup>
          <col className="w-14" />
          <col />
          <col className="w-16 sm:w-20" />
          <col className="w-16 sm:w-24" />
          <col className="hidden w-20 sm:table-column" />
          <col className="hidden w-32 lg:table-column" />
        </colgroup>
        <thead className="border-b border-line-strong">
          <tr>
            <th scope="col" className={cn(header, "pl-4 text-center")}>{t("official")}</th>
            <th scope="col" className={header}>{t("player")}</th>
            <th scope="col" className={cn(header, "text-center")}>{t("ours")}</th>
            <th scope="col" className={cn(header, "text-right")}>
              <span aria-hidden className="sm:hidden">
                Δ
              </span>
              <span className="sr-only sm:not-sr-only">{t("delta")}</span>
            </th>
            <th scope="col" className={cn(header, "hidden text-right sm:table-cell")}>{t("fss")}</th>
            <th scope="col" className={cn(header, "hidden pr-4 text-right lg:table-cell")}>{t("sample")}</th>
          </tr>
        </thead>
        <tbody>
          {entries.map((entry) => {
            const hasData = entry.status !== "noData";
            const name = (
              <span className="flex min-w-0 flex-col">
                <span className={cn("truncate font-semibold", hasData ? "text-main group-hover:text-gold-glow" : "text-secondary")}>
                  {entry.name}
                </span>
                <span className="truncate text-xs text-muted">
                  {[entry.club, entry.primaryPositionLabel].filter(Boolean).join(" · ")}
                </span>
              </span>
            );

            return (
              <tr key={`${entry.officialRank}-${entry.name}`} className="border-b border-line transition-colors hover:bg-elevated/60">
                <td className="py-3 pr-2 pl-4 text-center font-display text-lg font-black text-muted tabular-nums">
                  {entry.officialRank}
                </td>
                <td className="py-3 pr-3">
                  {entry.playerId && hasData ? (
                    <Link href={`/player/${entry.playerId}`} className="group flex items-center gap-3">
                      <PlayerAvatar name={entry.name} photoUrl={entry.photoUrl} />
                      {name}
                    </Link>
                  ) : (
                    <span className="flex items-center gap-3">
                      <PlayerAvatar name={entry.name} photoUrl={entry.photoUrl} />
                      {name}
                    </span>
                  )}
                </td>

                {entry.status === "ranked" && entry.ourRank !== null && entry.rankDelta !== null ? (
                  <>
                    <td className="py-3 text-center font-display text-lg font-black text-main tabular-nums">{entry.ourRank}</td>
                    <td className="py-3 pr-3 text-right">
                      <RankDelta delta={entry.rankDelta} />
                    </td>
                  </>
                ) : (
                  <td colSpan={2} className="py-3 pr-3 text-right">
                    <Badge tone="neutral" className="px-2 text-[10px] normal-case">
                      {t(`status.${entry.status === "notEligible" ? "notEligible" : "noData"}`)}
                    </Badge>
                  </td>
                )}

                <td className="hidden py-3 pr-3 text-right font-semibold text-main tabular-nums sm:table-cell">
                  {entry.fssScore === null ? (
                    <span className="font-normal text-muted">—</span>
                  ) : (
                    format.number(entry.fssScore, { minimumFractionDigits: 1, maximumFractionDigits: 1 })
                  )}
                </td>
                <td className="hidden py-3 pr-4 text-right text-sm text-secondary tabular-nums lg:table-cell">
                  {entry.totalMatches === null || entry.totalMinutes === null ? (
                    <span className="text-muted">—</span>
                  ) : (
                    t("sampleValue", { matches: entry.totalMatches, minutes: entry.totalMinutes })
                  )}
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
