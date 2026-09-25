import { useFormatter, useTranslations } from "next-intl";

import { MpsBadge } from "@/components/player/MpsBadge";
import { Card } from "@/components/ui/Card";
import type { AuditFormula } from "@/lib/types";

interface ReceiptCalculationProps {
  formula: AuditFormula;
  competitionName: string;
  algorithmVersion: number;
}

/**
 * A conta completa, na ordem do motor: MPS = 50 + (Σ ações − linha de base) × multiplicador de contexto,
 * limitado a 0–100. Os números vêm do recibo gravado; nada é recalculado no cliente além de exibir a conta.
 */
export function ReceiptCalculation({ formula, competitionName, algorithmVersion }: ReceiptCalculationProps) {
  const t = useTranslations("receipt");
  const format = useFormatter();
  const n = (value: number, digits = 2) => format.number(value, { minimumFractionDigits: 0, maximumFractionDigits: digits });
  const factor = (value: number) => `× ${format.number(value, { minimumFractionDigits: 2, maximumFractionDigits: 4 })}`;

  const unclamped = formula.baseScore + (formula.subtotalRaw - formula.positionBaseline) * formula.contextMultiplierCombined;
  const clamped = Math.abs(unclamped - formula.finalMps) > 0.01;

  const rows = [
    { label: t("baseScore"), value: n(formula.baseScore) },
    { label: t("subtotal"), value: n(formula.subtotalRaw) },
    { label: t("positionBaseline"), value: `− ${n(formula.positionBaseline)}`, hint: t("positionBaselineHint") },
  ];
  const multipliers = [
    { label: t("multipliers.tournament", { name: competitionName }), value: factor(formula.tournamentMultiplier) },
    { label: t("multipliers.opponent"), value: factor(formula.opponentMultiplier) },
    { label: t("multipliers.clutch"), value: factor(formula.clutchMultiplier) },
    { label: t("multipliers.combined"), value: factor(formula.contextMultiplierCombined), strong: true },
  ];

  return (
    <Card className="flex flex-col gap-6">
      <section className="flex flex-col gap-3">
        <h2 className="text-lg font-extrabold">{t("calculationTitle")}</h2>
        <dl className="flex flex-col gap-2">
          {rows.map((row) => (
            <div key={row.label} className="flex items-baseline justify-between gap-4" title={row.hint}>
              <dt className="text-sm text-secondary">{row.label}</dt>
              <dd className="font-semibold whitespace-nowrap text-main tabular-nums">{row.value}</dd>
            </div>
          ))}
        </dl>
      </section>

      <section className="flex flex-col gap-3">
        <h2 className="text-lg font-extrabold">{t("contextTitle")}</h2>
        <dl className="flex flex-col gap-2">
          {multipliers.map((row) => (
            <div key={row.label} className="flex items-baseline justify-between gap-4">
              <dt className={row.strong ? "text-sm font-semibold text-main" : "text-sm text-secondary"}>{row.label}</dt>
              <dd className="font-semibold whitespace-nowrap text-main tabular-nums">{row.value}</dd>
            </div>
          ))}
        </dl>
      </section>

      <section className="flex flex-col gap-3 border-t border-line pt-5">
        <p className="rounded-md bg-elevated px-3 py-2 font-mono text-sm text-secondary">
          {n(formula.baseScore)} + ({n(formula.subtotalRaw)} − {n(formula.positionBaseline)}) {factor(formula.contextMultiplierCombined)} ={" "}
          {n(unclamped)}
        </p>
        <div className="flex items-center justify-between gap-4">
          <span className="font-display text-lg font-bold">{t("finalScore")}</span>
          <MpsBadge value={formula.finalMps} size="lg" />
        </div>
        <ul className="flex flex-col gap-1 text-xs text-muted">
          {clamped && <li>{t("clamped")}</li>}
          <li>{formula.countsTowardsSeason ? t("countsTowardsSeason.yes") : t("countsTowardsSeason.no")}</li>
          <li>{t("minutesFactorHint")}</li>
          <li>{t("algorithmVersion", { version: algorithmVersion })}</li>
        </ul>
      </section>
    </Card>
  );
}
