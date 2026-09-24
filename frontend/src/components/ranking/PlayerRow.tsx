import { useFormatter, useTranslations } from "next-intl";
import Image from "next/image";

import { Badge } from "@/components/ui/Badge";
import { Link } from "@/i18n/navigation";
import type { SeasonRankingItem } from "@/lib/types";
import { cn } from "@/lib/utils";

import { PlayerAvatar } from "./PlayerAvatar";

interface PlayerRowProps {
  item: SeasonRankingItem;
  /** Posição exibida: a geral, ou a dentro da posição quando o ranking está filtrado. */
  rank: number;
}

const podium: Record<number, string> = {
  1: "text-gold-glow",
  2: "text-main",
  3: "text-gold-deep",
};

export function PlayerRow({ item, rank }: PlayerRowProps) {
  const format = useFormatter();
  const positions = useTranslations("positions");
  const decimal = (value: number) => format.number(value, { minimumFractionDigits: 1, maximumFractionDigits: 1 });
  const integer = (value: number) => format.number(value, { maximumFractionDigits: 0 });

  return (
    <tr className={cn("border-b border-line transition-colors hover:bg-elevated/60", rank === 1 && "bg-gold-subtle/40")}>
      <td className={cn("py-3 pr-2 pl-4 text-center font-display text-lg font-black tabular-nums", podium[rank] ?? "text-muted")}>
        {rank}
      </td>
      <td className="py-3 pr-3">
        <Link href={`/player/${item.playerId}`} className="group flex items-center gap-3">
          <PlayerAvatar name={item.playerName} photoUrl={item.photoUrl} highlighted={rank === 1} />
          <span className="flex min-w-0 flex-col">
            <span className="truncate font-semibold text-main group-hover:text-gold-glow">{item.playerName}</span>
            {item.teamName && (
              <span className="flex items-center gap-1.5 truncate text-xs text-muted md:hidden">
                {item.teamName}
              </span>
            )}
          </span>
        </Link>
      </td>
      <td className="hidden px-3 py-3 sm:table-cell">
        <Badge tone="neutral" title={item.primaryPositionLabel}>
          {positions(`short.${item.primaryPosition}`)}
        </Badge>
      </td>
      <td className="hidden px-3 py-3 md:table-cell">
        {item.teamName && (
          <span className="flex items-center gap-2 text-sm text-secondary">
            {item.teamLogoUrl && <Image src={item.teamLogoUrl} alt="" width={20} height={20} className="shrink-0" />}
            <span className="truncate">{item.teamName}</span>
          </span>
        )}
      </td>
      <td className="hidden px-3 py-3 text-right text-sm text-secondary tabular-nums lg:table-cell">{integer(item.totalMatches)}</td>
      <td className="hidden px-3 py-3 text-right text-sm text-secondary tabular-nums lg:table-cell">{integer(item.totalMinutes)}</td>
      <td className="hidden px-3 py-3 text-right text-sm text-secondary tabular-nums sm:table-cell">{decimal(item.mpsAverage)}</td>
      <td className="py-3 pr-4 pl-3 text-right font-display text-lg font-extrabold text-gold-glow tabular-nums">
        {decimal(item.fssScore)}
      </td>
    </tr>
  );
}
