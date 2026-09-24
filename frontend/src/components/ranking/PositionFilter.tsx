import { useTranslations } from "next-intl";

import { Link } from "@/i18n/navigation";
import { playerPositions } from "@/lib/types";
import { cn } from "@/lib/utils";

import { rankingHref, type RankingQuery } from "./rankingQuery";

interface PositionFilterProps {
  query: RankingQuery;
}

export function PositionFilter({ query }: PositionFilterProps) {
  const t = useTranslations("ranking.filters");
  const positions = useTranslations("positions");

  const options = [
    { value: undefined, label: t("all"), title: t("all") },
    ...playerPositions.map((p) => ({ value: p, label: positions(`short.${p}`), title: positions(`names.${p}`) })),
  ];

  return (
    <nav aria-label={t("position")} className="-mx-4 overflow-x-auto px-4 sm:mx-0 sm:px-0">
      <ul className="flex gap-2">
        {options.map(({ value, label, title }) => {
          const active = query.position === value;

          return (
            <li key={value ?? "all"}>
              <Link
                href={rankingHref(query, { position: value, page: 1 })}
                title={title}
                aria-current={active ? "page" : undefined}
                className={cn(
                  "block rounded-full border px-3.5 py-1.5 text-sm font-semibold whitespace-nowrap transition-colors duration-[var(--trb-duration)]",
                  active
                    ? "border-gold-border bg-gold-subtle text-gold-glow"
                    : "border-line text-secondary hover:border-line-strong hover:text-main",
                )}
              >
                {label}
              </Link>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
