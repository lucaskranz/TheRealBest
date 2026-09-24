import { useTranslations } from "next-intl";

import { buttonStyles } from "@/components/ui/Button";
import { Link } from "@/i18n/navigation";

import { rankingHref, type RankingQuery } from "./rankingQuery";

interface RankingPaginationProps {
  query: RankingQuery;
  totalPages: number;
}

export function RankingPagination({ query, totalPages }: RankingPaginationProps) {
  const t = useTranslations("ranking.pagination");

  if (totalPages <= 1) {
    return null;
  }

  const disabled = "pointer-events-none opacity-40";

  return (
    <nav aria-label={t("label")} className="flex items-center justify-between gap-4">
      <Link
        href={rankingHref(query, { page: query.page - 1 })}
        aria-disabled={query.page <= 1}
        className={buttonStyles("secondary", "sm", query.page <= 1 ? disabled : undefined)}
      >
        {t("previous")}
      </Link>
      <span className="text-sm text-muted">{t("status", { page: query.page, total: totalPages })}</span>
      <Link
        href={rankingHref(query, { page: query.page + 1 })}
        aria-disabled={query.page >= totalPages}
        className={buttonStyles("secondary", "sm", query.page >= totalPages ? disabled : undefined)}
      >
        {t("next")}
      </Link>
    </nav>
  );
}
