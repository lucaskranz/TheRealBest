import type { ReactNode } from "react";

import { Card } from "@/components/ui/Card";
import { cn } from "@/lib/utils";

interface RankingMessageProps {
  title: string;
  description?: string;
  tone?: "neutral" | "danger";
  children?: ReactNode;
}

/** Estado vazio ou de erro no lugar da tabela. */
export function RankingMessage({ title, description, tone = "neutral", children }: RankingMessageProps) {
  return (
    <Card className={cn("flex flex-col items-center gap-3 py-12 text-center", tone === "danger" && "border-danger/30")}>
      <p className={cn("font-display text-xl font-bold", tone === "danger" ? "text-danger" : "text-main")}>{title}</p>
      {description && <p className="max-w-md text-sm text-secondary">{description}</p>}
      {children}
    </Card>
  );
}
