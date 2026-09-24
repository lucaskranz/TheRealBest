import { useTranslations } from "next-intl";

import { Badge } from "@/components/ui/Badge";
import { buttonStyles } from "@/components/ui/Button";
import { Link } from "@/i18n/navigation";

export function Hero() {
  const t = useTranslations("common.home.hero");

  return (
    <section className="mx-auto flex max-w-6xl flex-col items-start gap-8 px-4 pt-16 pb-12 sm:px-6 md:pt-24">
      <Badge tone="gold">
        <span aria-hidden>✦</span>
        {t("tag")}
      </Badge>

      <h1 className="max-w-3xl text-4xl leading-[1.05] font-black sm:text-5xl md:text-6xl">
        {t("title")}
        <br />
        <span className="text-gradient-gold">{t("titleHighlight")}</span>
      </h1>

      <p className="max-w-2xl text-lg text-secondary">{t("subtitle")}</p>

      <div className="flex flex-wrap gap-3">
        <Link href="/ranking" className={buttonStyles("primary", "lg")}>
          {t("primaryCta")}
        </Link>
        <Link href="/formula" className={buttonStyles("secondary", "lg")}>
          {t("secondaryCta")}
        </Link>
      </div>
    </section>
  );
}
