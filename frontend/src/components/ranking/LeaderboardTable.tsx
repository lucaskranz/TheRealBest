import { useTranslations } from "next-intl";

import type { SeasonRankingItem } from "@/lib/types";

import { PlayerRow } from "./PlayerRow";

interface LeaderboardTableProps {
  items: SeasonRankingItem[];
  caption: string;
  /** Com filtro de posição, mostra a posição do jogador dentro dela. */
  byPosition: boolean;
}

export function LeaderboardTable({ items, caption, byPosition }: LeaderboardTableProps) {
  const t = useTranslations("ranking");
  const header = "px-3 py-3 text-xs font-semibold tracking-wide text-muted uppercase";

  return (
    <div className="glass overflow-hidden rounded-lg shadow-card">
      <table className="w-full table-fixed border-collapse text-left">
        <caption className="sr-only">{caption}</caption>
        <colgroup>
          <col className="w-14" />
          <col />
          <col className="hidden w-24 sm:table-column" />
          <col className="hidden w-48 md:table-column" />
          <col className="hidden w-20 lg:table-column" />
          <col className="hidden w-24 lg:table-column" />
          <col className="hidden w-24 sm:table-column" />
          <col className="w-20 sm:w-24" />
        </colgroup>
        <thead className="border-b border-line-strong">
          <tr>
            <th scope="col" className={`${header} pl-4 text-center`}>{t("columns.rank")}</th>
            <th scope="col" className={header}>{t("columns.player")}</th>
            <th scope="col" className={`${header} hidden sm:table-cell`}>{t("columns.position")}</th>
            <th scope="col" className={`${header} hidden md:table-cell`}>{t("columns.team")}</th>
            <th scope="col" className={`${header} hidden text-right lg:table-cell`}>{t("columns.matches")}</th>
            <th scope="col" className={`${header} hidden text-right lg:table-cell`}>{t("columns.minutes")}</th>
            <th scope="col" className={`${header} hidden text-right sm:table-cell`}>
              <abbr title={t("hints.mpsAverage")} className="no-underline">{t("columns.mpsAverage")}</abbr>
            </th>
            <th scope="col" className={`${header} pr-4 text-right`}>
              <abbr title={t("hints.fss")} className="no-underline">{t("columns.fss")}</abbr>
            </th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <PlayerRow key={item.playerId} item={item} rank={byPosition ? item.positionRank : item.overallRank} />
          ))}
        </tbody>
      </table>
    </div>
  );
}
