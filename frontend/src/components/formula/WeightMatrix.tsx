import { useFormatter, useTranslations } from "next-intl";

import type { FormulaAction, FormulaPosition } from "@/lib/types";
import { cn } from "@/lib/utils";

interface WeightMatrixProps {
  actions: FormulaAction[];
  positions: FormulaPosition[];
  version: number;
}

/** Opacidade máxima do fundo da célula: o número continua legível e na cor do texto. */
const MAX_TINT = 35;

/**
 * Matriz ação × posição. O fundo de cada célula é proporcional ao maior peso da linha, para mostrar
 * em que função a ação mais pesa (azul = soma, vermelho = desconta). Os números ficam na cor do texto.
 */
export function WeightMatrix({ actions, positions, version }: WeightMatrixProps) {
  const t = useTranslations("formula.weights");
  const format = useFormatter();
  const weight = (value: number) => {
    const text = format.number(Math.abs(value), { maximumFractionDigits: 2 });
    return value < 0 ? `−${text}` : text;
  };

  const groups = [
    { key: "positive", title: t("positive"), rows: actions.filter((a) => !a.isPenalty) },
    { key: "penalties", title: t("penalties"), rows: actions.filter((a) => a.isPenalty) },
  ];
  const header = "px-2 py-3 text-xs font-semibold tracking-wide text-muted uppercase";

  return (
    <div className="glass overflow-x-auto rounded-lg shadow-card">
      <table className="w-full min-w-[720px] border-collapse text-left text-sm">
        <caption className="sr-only">{t("caption", { version })}</caption>
        <thead className="border-b border-line-strong">
          <tr>
            <th scope="col" className={cn(header, "sticky left-0 z-10 bg-surface pl-4")}>
              {t("action")}
            </th>
            {positions.map((position) => (
              <th key={position.code} scope="col" className={cn(header, "w-16 text-right")}>
                <abbr title={position.label} className="no-underline">
                  {position.code}
                </abbr>
              </th>
            ))}
          </tr>
        </thead>

        {groups.map((group) => (
          <tbody key={group.key} className="border-b border-line-strong">
            <tr>
              <th
                scope="colgroup"
                colSpan={positions.length + 1}
                className="sticky left-0 bg-elevated/60 px-4 py-2 text-xs font-bold tracking-wide text-secondary uppercase"
              >
                {group.title}
              </th>
            </tr>
            {group.rows.map((action) => {
              const rowMax = Math.max(...Object.values(action.weights).map((w) => Math.abs(w ?? 0)));

              return (
                <tr key={action.key} className={cn("border-b border-line", !action.hasDataSource && "opacity-55")}>
                  <th scope="row" className="sticky left-0 z-10 bg-surface py-2 pr-2 pl-4 font-normal">
                    <span className="flex flex-wrap items-baseline gap-x-2">
                      <span className="font-medium text-main">{action.label}</span>
                      {action.scalesWithMinutes && (
                        <span className="text-xs text-muted" title={t("volumeLegend")}>
                          ∝
                        </span>
                      )}
                      {action.isDecisive && (
                        <span className="text-xs text-gold-glow" title={t("decisiveLegend")}>
                          ★
                        </span>
                      )}
                      {!action.hasDataSource && (
                        <span className="rounded-sm border border-line px-1 text-[10px] tracking-wide text-muted uppercase">
                          {t("noData")}
                        </span>
                      )}
                    </span>
                    <span className="block text-xs text-muted">{t(`categories.${action.category}`)}</span>
                  </th>
                  {positions.map((position) => {
                    const value = action.weights[position.code];
                    const tint = value && rowMax > 0 ? Math.round((Math.abs(value) / rowMax) * MAX_TINT) : 0;
                    const color = value !== undefined && value < 0 ? "var(--trb-below)" : "var(--trb-above)";

                    return (
                      <td
                        key={position.code}
                        className="px-2 py-2 text-right font-semibold whitespace-nowrap text-main tabular-nums"
                        style={tint > 0 ? { backgroundColor: `color-mix(in oklab, ${color} ${tint}%, transparent)` } : undefined}
                      >
                        {value === undefined ? <span className="font-normal text-muted">—</span> : weight(value)}
                      </td>
                    );
                  })}
                </tr>
              );
            })}
          </tbody>
        ))}

        <tfoot>
          <tr>
            <th scope="row" className="sticky left-0 z-10 bg-surface py-3 pr-2 pl-4 font-normal" title={t("baselineHint")}>
              <span className="font-semibold text-main">{t("baseline")}</span>
              <span className="block text-xs text-muted">{t("baselineHint")}</span>
            </th>
            {positions.map((position) => (
              <td key={position.code} className="px-2 py-3 text-right font-bold whitespace-nowrap text-main tabular-nums">
                {format.number(position.baseline, { minimumFractionDigits: 1, maximumFractionDigits: 1 })}
              </td>
            ))}
          </tr>
        </tfoot>
      </table>
    </div>
  );
}
