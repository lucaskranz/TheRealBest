"use client";

import { useLocale, useTranslations } from "next-intl";
import { useTransition } from "react";

import { usePathname, useRouter } from "@/i18n/navigation";
import { routing, type Locale } from "@/i18n/routing";
import { cn } from "@/lib/utils";

const shortCodes: Record<Locale, string> = {
  "pt-BR": "PT",
  en: "EN",
  es: "ES",
};

export function LocaleSwitcher() {
  const t = useTranslations("common.localeSwitcher");
  const currentLocale = useLocale();
  const router = useRouter();
  const pathname = usePathname();
  const [isPending, startTransition] = useTransition();

  function switchTo(locale: Locale) {
    startTransition(() => {
      // O proxy do next-intl grava o cookie NEXT_LOCALE ao visitar a rota do novo locale
      router.replace(pathname, { locale });
    });
  }

  return (
    <div
      role="group"
      aria-label={t("label")}
      className={cn("glass flex items-center rounded-full p-1", isPending && "opacity-60")}
    >
      {routing.locales.map((locale) => {
        const active = locale === currentLocale;

        return (
          <button
            key={locale}
            type="button"
            lang={locale}
            title={t(`locales.${locale}`)}
            aria-label={t(`locales.${locale}`)}
            aria-pressed={active}
            disabled={active || isPending}
            onClick={() => switchTo(locale)}
            className={cn(
              "rounded-full px-2.5 py-1 text-xs font-bold tracking-wide transition-colors duration-[var(--trb-duration)]",
              active ? "bg-gold text-on-gold" : "text-secondary hover:text-main",
            )}
          >
            {shortCodes[locale]}
          </button>
        );
      })}
    </div>
  );
}
