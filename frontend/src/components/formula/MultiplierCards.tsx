import { useFormatter, useTranslations } from "next-intl";

import { Card } from "@/components/ui/Card";
import type { Formula } from "@/lib/types";

interface MultiplierCardsProps {
  formula: Formula;
}

interface Row {
  key: string;
  label: string;
  value: number;
}

/** Torneio, adversário e fator decisivo: cada faixa com o multiplicador exato do motor. */
export function MultiplierCards({ formula }: MultiplierCardsProps) {
  const t = useTranslations("formula.multipliers");

  const midTableElo = formula.opponentWeights.find((w) => w.key === "midTable")?.minElo ?? null;

  const tournament: Row[] = formula.tournamentWeights.map((w) => ({
    key: w.key,
    label: t(`tournament.${w.key}`),
    value: w.value,
  }));

  const opponent: Row[] = formula.opponentWeights.map((w) => ({
    key: w.key,
    label: t(`opponent.${w.key}`, { elo: String(w.minElo ?? midTableElo ?? "") }),
    value: w.value,
  }));

  const clutch: Row[] = formula.clutchWeights.map((w) => ({
    key: w.key,
    label: t(`clutch.${w.key}`, { min: w.minMargin, max: w.maxMargin ?? w.minMargin }),
    value: w.value,
  }));

  return (
    <div className="grid gap-4 lg:grid-cols-3">
      <MultiplierCard title={t("tournament.title")} rows={tournament} />
      <MultiplierCard title={t("opponent.title")} description={t("opponent.description")} rows={opponent} />
      <MultiplierCard title={t("clutch.title")} description={t("clutch.description")} rows={clutch} />
    </div>
  );
}

function MultiplierCard({ title, description, rows }: { title: string; description?: string; rows: Row[] }) {
  const format = useFormatter();

  return (
    <Card className="flex flex-col gap-4">
      <header className="flex flex-col gap-1">
        <h3 className="text-lg font-extrabold">{title}</h3>
        {description && <p className="text-sm text-muted">{description}</p>}
      </header>
      <dl className="flex flex-col gap-2">
        {rows.map((row) => (
          <div key={row.key} className="flex items-baseline justify-between gap-4 border-b border-line pb-2 last:border-b-0">
            <dt className="text-sm text-secondary">{row.label}</dt>
            <dd className="font-semibold whitespace-nowrap text-main tabular-nums">
              × {format.number(row.value, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
            </dd>
          </div>
        ))}
      </dl>
    </Card>
  );
}
