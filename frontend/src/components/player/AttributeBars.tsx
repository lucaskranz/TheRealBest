import { useFormatter, useTranslations } from "next-intl";

import { Card } from "@/components/ui/Card";
import type { PlayerAttribute } from "@/lib/types";

interface AttributeBarsProps {
  attributes: PlayerAttribute[];
  positionLabel: string;
  /** Espelha PlayerAttributeCalculator.MinMinutesForComparison no backend. */
  minMinutes: number;
}

/**
 * Percentil por dimensão entre jogadores da mesma posição: uma série, uma cor (dourado da marca) sobre uma
 * trilha do mesmo tom. O valor fica escrito ao lado; pontos por 90 minutos no texto de apoio.
 */
export function AttributeBars({ attributes, positionLabel, minMinutes }: AttributeBarsProps) {
  const t = useTranslations("player.attributes");
  const format = useFormatter();

  if (attributes.length === 0) {
    return null;
  }

  const peers = attributes[0].peerCount;
  const hasPercentiles = attributes.some((a) => a.percentile !== null);

  return (
    <Card className="flex flex-col gap-5">
      <header className="flex flex-col gap-1">
        <h2 className="text-xl font-extrabold">{t("title")}</h2>
        <p className="text-sm text-secondary">
          {hasPercentiles
            ? t("description", { peers, minutes: minMinutes })
            : t("fewPeers", { peers })}{" "}
          <span className="text-muted">({positionLabel})</span>
        </p>
      </header>

      <ul className="flex flex-col gap-4">
        {attributes.map((attribute) => {
          const per90 = t("per90", {
            value: format.number(attribute.per90, { minimumFractionDigits: 1, maximumFractionDigits: 1, signDisplay: "exceptZero" }),
          });

          return (
            <li
              key={attribute.category}
              className={hasPercentiles ? "grid grid-cols-[minmax(0,9rem)_1fr_2.5rem] items-center gap-3" : "flex items-baseline justify-between gap-3"}
            >
              <span className="flex flex-col">
                <span className="text-sm font-semibold text-main">{t(`categories.${attribute.category}`)}</span>
                <span className="text-xs text-muted">{per90}</span>
              </span>
              {hasPercentiles && (
                <span
                  className="h-2.5 overflow-hidden rounded-full bg-gold-subtle"
                  role="img"
                  aria-label={`${t(`categories.${attribute.category}`)}: ${attribute.percentile ?? "—"}`}
                  title={per90}
                >
                  {attribute.percentile !== null && (
                    <span
                      className="block h-full rounded-r-[4px] bg-gold transition-[width] duration-500 ease-out-soft"
                      style={{ width: `${Math.max(attribute.percentile, 2)}%` }}
                    />
                  )}
                </span>
              )}
              {hasPercentiles && (
                <span className="text-right font-display text-lg font-bold text-main tabular-nums">
                  {attribute.percentile ?? "—"}
                </span>
              )}
            </li>
          );
        })}
      </ul>
    </Card>
  );
}
