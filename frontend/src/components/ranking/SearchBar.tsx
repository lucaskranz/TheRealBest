import { useLocale, useTranslations } from "next-intl";
import Form from "next/form";

import { buttonStyles } from "@/components/ui/Button";
import { getPathname, Link } from "@/i18n/navigation";

import { rankingHref, type RankingQuery } from "./rankingQuery";

interface SearchBarProps {
  query: RankingQuery;
}

/** Busca por nome. Envia GET para a própria página (?search=), preservando o filtro de posição. */
export function SearchBar({ query }: SearchBarProps) {
  const t = useTranslations("ranking.filters");
  const locale = useLocale();

  return (
    <Form action={getPathname({ locale, href: "/ranking" })} role="search" className="flex w-full gap-2 sm:w-auto">
      {query.position && <input type="hidden" name="position" value={query.position} />}
      <label className="sr-only" htmlFor="ranking-search">
        {t("search")}
      </label>
      <input
        id="ranking-search"
        name="search"
        type="search"
        defaultValue={query.search}
        placeholder={t("searchPlaceholder")}
        maxLength={60}
        className="glass h-11 min-w-0 flex-1 rounded-md px-4 text-sm text-main placeholder:text-muted focus:border-gold-border focus:outline-none sm:w-64"
      />
      <button type="submit" className={buttonStyles("secondary", "md")}>
        {t("submit")}
      </button>
      {query.search && (
        <Link href={rankingHref(query, { search: undefined, page: 1 })} className={buttonStyles("ghost", "md")}>
          {t("clear")}
        </Link>
      )}
    </Form>
  );
}
