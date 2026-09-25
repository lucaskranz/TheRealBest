import { useFormatter, useTranslations } from "next-intl";

import type { AuditActionItem } from "@/lib/types";

interface ReceiptItemsProps {
  title: string;
  emptyText: string;
  items: AuditActionItem[];
}

/** Linhas do recibo: quantidade × peso (× fator de minutos) = pontos. A marca lateral indica o sinal. */
export function ReceiptItems({ title, emptyText, items }: ReceiptItemsProps) {
  const t = useTranslations("receipt");
  const format = useFormatter();
  const number = (value: number, digits = 2) => format.number(value, { maximumFractionDigits: digits });
  const points = (value: number) =>
    format.number(value, { minimumFractionDigits: 1, maximumFractionDigits: 2, signDisplay: "exceptZero" });

  const sorted = [...items].sort((a, b) => Math.abs(b.totalPoints) - Math.abs(a.totalPoints));

  return (
    <section className="flex flex-col gap-3">
      <h2 className="text-lg font-extrabold">{title}</h2>
      {sorted.length === 0 ? (
        <p className="text-sm text-muted">{emptyText}</p>
      ) : (
        <ul className="flex flex-col divide-y divide-line">
          {sorted.map((item) => (
            <li key={item.actionKey} className="flex items-center gap-3 py-2.5">
              <span
                aria-hidden
                className="h-8 w-1 shrink-0 rounded-full"
                style={{ background: item.totalPoints >= 0 ? "var(--trb-above)" : "var(--trb-below)" }}
              />
              <span className="flex min-w-0 flex-1 flex-col">
                <span className="font-semibold text-main">{item.label}</span>
                <span className="text-xs text-muted tabular-nums">
                  {number(item.count)} × {number(item.unitWeight)}
                  {item.minutesFactor !== 1 && ` · ${t("minutesFactor", { factor: number(item.minutesFactor, 4) })}`}
                </span>
              </span>
              <span className="font-display text-lg font-bold text-main tabular-nums">{points(item.totalPoints)}</span>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
