import { useFormatter, useTranslations } from "next-intl";

import { Link } from "@/i18n/navigation";
import { matchTitle } from "@/lib/format";
import type { PlayerMatchStat } from "@/lib/types";
import { cn } from "@/lib/utils";

import { MpsBadge } from "./MpsBadge";

interface MatchListProps {
  playerId: string;
  matches: PlayerMatchStat[];
}

export function MatchList({ playerId, matches }: MatchListProps) {
  const t = useTranslations("player.matches");
  const positions = useTranslations("positions");
  const format = useFormatter();
  const header = "px-3 py-3 text-xs font-semibold tracking-wide text-muted uppercase";

  return (
    <section aria-labelledby="matches-title" className="flex flex-col gap-4">
      <div className="flex flex-col gap-1">
        <h2 id="matches-title" className="text-xl font-extrabold">{t("title")}</h2>
        <p className="text-sm text-muted">{t("legend")}</p>
      </div>

      {matches.length === 0 ? (
        <p className="text-secondary">{t("empty")}</p>
      ) : (
        <div className="glass overflow-hidden rounded-lg shadow-card">
          <table className="w-full border-collapse text-left">
            <thead className="border-b border-line-strong">
              <tr>
                <th scope="col" className={`${header} hidden pl-4 sm:table-cell`}>{t("columns.date")}</th>
                <th scope="col" className={`${header} pl-4 sm:pl-3`}>{t("columns.match")}</th>
                <th scope="col" className={`${header} hidden text-right md:table-cell`}>{t("columns.minutes")}</th>
                <th scope="col" className={`${header} hidden text-right sm:table-cell`}>{t("columns.contributions")}</th>
                <th scope="col" className={`${header} pr-4 text-right`}>{t("columns.mps")}</th>
              </tr>
            </thead>
            <tbody>
              {matches.map((m) => (
                <tr key={m.matchId} className={cn("border-b border-line last:border-0", !m.countsTowardsSeason && "opacity-60")}>
                  <td className="hidden py-3 pr-3 pl-4 text-sm text-secondary tabular-nums sm:table-cell">
                    {format.dateTime(new Date(m.matchDate), { day: "2-digit", month: "short", year: "numeric" })}
                  </td>
                  <td className="py-3 pr-3 pl-4 sm:pl-3">
                    <Link href={`/player/${playerId}/match/${m.matchId}`} className="group flex flex-col">
                      <span className="font-semibold text-main group-hover:text-gold-glow">
                        {matchTitle(m.homeTeamName, m.awayTeamName, m.homeScore, m.awayScore)}
                      </span>
                      <span className="text-xs text-muted">
                        {m.competitionName} · {positions(`short.${m.positionPlayed}`)}
                        {!m.countsTowardsSeason && ` · ${t("notCounted")}`}
                      </span>
                    </Link>
                  </td>
                  <td className="hidden px-3 py-3 text-right text-sm text-secondary tabular-nums md:table-cell">{m.minutesPlayed}′</td>
                  <td className="hidden px-3 py-3 text-right text-sm text-secondary tabular-nums sm:table-cell">
                    {m.goals}/{m.assists}
                  </td>
                  <td className="py-3 pr-4 pl-3 text-right">
                    <Link href={`/player/${playerId}/match/${m.matchId}`} aria-label={t("viewReceipt")} title={t("viewReceipt")}>
                      <MpsBadge value={m.finalMps} />
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
