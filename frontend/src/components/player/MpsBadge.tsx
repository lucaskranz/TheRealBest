import { useFormatter } from "next-intl";

import { cn } from "@/lib/utils";

interface MpsBadgeProps {
  value: number | null;
  size?: "sm" | "lg";
}

/**
 * MPS com escala divergente em torno de 50 (atuação média): azul acima, vermelho abaixo, cinza perto do meio,
 * em três passos iguais por lado. O número fica em cor de texto; a cor só marca a polaridade.
 */
export function MpsBadge({ value, size = "sm" }: MpsBadgeProps) {
  const format = useFormatter();

  if (value === null) {
    return <span className="text-muted">—</span>;
  }

  const distance = value - 50;
  const step = Math.abs(distance) < 5 ? 0 : Math.abs(distance) < 15 ? 1 : Math.abs(distance) < 30 ? 2 : 3;
  const pole = distance >= 0 ? "var(--trb-above)" : "var(--trb-below)";
  const background = step === 0
    ? "var(--trb-neutral-mark)"
    : `color-mix(in oklab, ${pole} ${[0, 30, 50, 75][step]}%, var(--trb-bg-elevated))`;

  return (
    <span
      style={{ background }}
      className={cn(
        "inline-flex items-center justify-center rounded-md font-display font-extrabold text-main tabular-nums",
        size === "sm" ? "h-7 min-w-12 px-2 text-sm" : "h-12 min-w-20 px-3 text-2xl",
      )}
    >
      {format.number(value, { minimumFractionDigits: 1, maximumFractionDigits: 1 })}
    </span>
  );
}
