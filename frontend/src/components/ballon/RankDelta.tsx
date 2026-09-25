import { useTranslations } from "next-intl";

import { cn } from "@/lib/utils";

interface RankDeltaProps {
  /** Colocação do júri menos a do índice: positivo = o índice coloca o jogador mais alto. */
  delta: number;
}

/** Só a seta leva cor (azul acima, vermelho abaixo); o número fica na cor do texto. */
export function RankDelta({ delta }: RankDeltaProps) {
  const t = useTranslations("ballon.table");
  const count = Math.abs(delta);
  const label = delta > 0 ? t("deltaUp", { count }) : delta < 0 ? t("deltaDown", { count }) : t("deltaSame");

  return (
    <span className="inline-flex items-center justify-end gap-1 font-semibold text-main tabular-nums" title={label}>
      <span aria-hidden className={cn("text-xs", delta > 0 ? "text-above" : delta < 0 ? "text-below" : "text-muted")}>
        {delta > 0 ? "▲" : delta < 0 ? "▼" : "="}
      </span>
      <span aria-hidden>{delta === 0 ? "0" : count}</span>
      <span className="sr-only">{label}</span>
    </span>
  );
}
