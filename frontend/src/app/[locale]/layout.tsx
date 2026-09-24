import type { Metadata } from "next";
import { hasLocale, NextIntlClientProvider } from "next-intl";
import { getTranslations, setRequestLocale } from "next-intl/server";
import { notFound } from "next/navigation";

import { Footer } from "@/components/layout/Footer";
import { Header } from "@/components/layout/Header";
import { routing } from "@/i18n/routing";
import { cabinetGrotesk, inter } from "@/lib/fonts";
import { siteUrl } from "@/lib/seo";
import { cn } from "@/lib/utils";

export function generateStaticParams() {
  return routing.locales.map((locale) => ({ locale }));
}

export async function generateMetadata({ params }: LayoutProps<"/[locale]">): Promise<Metadata> {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) return {};

  const t = await getTranslations({ locale, namespace: "common.metadata" });

  return {
    metadataBase: new URL(siteUrl),
    title: { default: t("title"), template: `%s | ${t("title")}` },
    description: t("description"),
    openGraph: {
      type: "website",
      siteName: t("title"),
      locale: locale.replace("-", "_"),
      alternateLocale: routing.locales.filter((l) => l !== locale).map((l) => l.replace("-", "_")),
    },
  };
}

export default async function LocaleLayout({ children, params }: LayoutProps<"/[locale]">) {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) {
    notFound();
  }

  // Permite renderização estática das páginas localizadas
  setRequestLocale(locale);

  return (
    <html lang={locale} className={cn(cabinetGrotesk.variable, inter.variable)}>
      <body className="flex min-h-dvh flex-col">
        <NextIntlClientProvider>
          <Header />
          <main className="flex-1">{children}</main>
          <Footer />
        </NextIntlClientProvider>
      </body>
    </html>
  );
}
