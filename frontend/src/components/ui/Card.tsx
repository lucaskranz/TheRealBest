import type { HTMLAttributes } from "react";

import { cn } from "@/lib/utils";

export interface CardProps extends HTMLAttributes<HTMLDivElement> {
  /** Destaca o card com borda e brilho dourados (ex.: líder do ranking). */
  highlighted?: boolean;
  /** Eleva o card no hover — para cards clicáveis. */
  interactive?: boolean;
}

export function Card({ highlighted = false, interactive = false, className, ...props }: CardProps) {
  return (
    <div
      className={cn(
        "glass rounded-lg p-6 shadow-card",
        highlighted && "border-gold-border shadow-glow",
        interactive &&
          "transition-all duration-[var(--trb-duration)] ease-out-soft hover:-translate-y-1 hover:border-line-strong",
        className,
      )}
      {...props}
    />
  );
}
