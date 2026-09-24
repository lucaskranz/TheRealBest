import { useTranslations } from "next-intl";

import { buttonStyles } from "@/components/ui/Button";
import { Link } from "@/i18n/navigation";

export default function LocaleNotFound() {
  const t = useTranslations("common.notFound");

  return (
    <section className="mx-auto flex max-w-xl flex-col items-center gap-6 px-4 py-32 text-center">
      <p className="font-display text-7xl font-black text-gradient-gold">404</p>
      <h1 className="text-3xl font-extrabold">{t("title")}</h1>
      <p className="text-secondary">{t("description")}</p>
      <Link href="/" className={buttonStyles("secondary")}>
        {t("backHome")}
      </Link>
    </section>
  );
}
