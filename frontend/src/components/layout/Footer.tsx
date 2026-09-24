import { useTranslations } from "next-intl";

export function Footer() {
  const t = useTranslations("common");

  return (
    <footer className="mt-24 border-t border-line">
      <div className="mx-auto flex max-w-6xl flex-col gap-3 px-4 py-10 text-sm text-muted sm:px-6">
        <p className="font-display text-base font-bold text-secondary">{t("brand.slogan")}</p>
        <p>{t("footer.coverage")}</p>
        <p>{t("footer.dataSources")}</p>
        <p>{t("footer.rights", { year: new Date().getFullYear() })}</p>
      </div>
    </footer>
  );
}
