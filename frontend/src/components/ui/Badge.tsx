import type { HTMLAttributes } from "react";

import { cn } from "@/lib/utils";

export type BadgeTone = "gold" | "emerald" | "danger" | "neutral";

const tones: Record<BadgeTone, string> = {
  gold: "bg-gold-subtle text-gold-glow border-gold-border",
  emerald: "bg-emerald-subtle text-emerald border-emerald/30",
  danger: "bg-danger-subtle text-danger border-danger/30",
  neutral: "bg-elevated text-secondary border-line",
};

export interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  tone?: BadgeTone;
}

export function Badge({ tone = "neutral", className, ...props }: BadgeProps) {
  return (
    <span
      className={cn(
        "inline-flex items-center gap-1.5 rounded-full border px-3 py-1 text-xs font-semibold tracking-wide uppercase",
        tones[tone],
        className,
      )}
      {...props}
    />
  );
}
