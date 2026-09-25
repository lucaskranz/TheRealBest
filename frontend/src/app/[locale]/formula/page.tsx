import type { Metadata } from "next";
import { hasLocale } from "next-intl";
import { getFormatter, getTranslations, setRequestLocale } from "next-intl/server";
import { notFound } from "next/navigation";

import { MultiplierCards } from "@/components/formula/MultiplierCards";
import { WeightMatrix } from "@/components/formula/WeightMatrix";
import { RankingMessage } from "@/components/ranking/RankingMessage";
import { Badge } from "@/components/ui/Badge";
import { buttonStyles } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { Link } from "@/i18n/navigation";
import { routing, type Locale } from "@/i18n/routing";
import { apiGet } from "@/lib/api";
import { buildAlternates } from "@/lib/seo";
import type { Formula } from "@/lib/types";

export async function generateMetadata({ params }: PageProps<"/[locale]/formula">): Promise<Metadata> {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) return {};

  const t = await getTranslations({ locale, namespace: "formula.metadata" });

  return {
    title: t("title"),
    description: t("description"),
    alternates: buildAlternates(locale, "/formula"),
  };
}

async function loadFormula(locale: Locale): Promise<Formula | null> {
  try {
    return (await apiGet<Formula>("/formula", { locale })).data ?? null;
  } catch (error) {
    console.error("Failed to load formula", error);
    return null;
  }
}

export default async function FormulaPage({ params }: PageProps<"/[locale]/formula">) {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) notFound();

  setRequestLocale(locale);

  const [t, format, formula] = await Promise.all([getTranslations("formula"), getFormatter(), loadFormula(locale)]);

  if (formula === null) {
    return (
      <section className="mx-auto flex max-w-6xl flex-col gap-6 px-4 py-12 sm:px-6">
        <h1 className="text-3xl font-black sm:text-4xl">{t("title")}</h1>
        <RankingMessage tone="danger" title={t("error.title")} description={t("error.description")} />
      </section>
    );
  }

  const n = (value: number) => format.number(value, { maximumFractionDigits: 2 });
  const { mps, minutes, season } = formula;
  const sharedPositions = formula.positions.filter((p) => p.sharesMatrixWith !== null);

  const steps = ["actions", "context", "season"] as const;

  return (
    <section className="mx-auto flex max-w-6xl flex-col gap-12 px-4 py-12 sm:px-6">
      <header className="flex flex-col items-start gap-3">
        <Badge tone="gold">{t("version", { version: formula.algorithmVersion })}</Badge>
        <h1 className="text-3xl font-black sm:text-4xl">{t("title")}</h1>
        <p className="max-w-3xl text-secondary">{t("subtitle")}</p>
      </header>

      <section aria-labelledby="steps-title" className="flex flex-col gap-4">
        <h2 id="steps-title" className="sr-only">
          {t("steps.title")}
        </h2>
        <ol className="grid gap-4 md:grid-cols-3">
          {steps.map((step) => (
            <li key={step}>
              <Card className="flex h-full flex-col gap-2">
                <h3 className="text-lg font-extrabold">{t(`steps.${step}.title`)}</h3>
                <p className="text-sm text-secondary">{t(`steps.${step}.description`, { base: n(mps.baseScore) })}</p>
              </Card>
            </li>
          ))}
        </ol>
      </section>

      <div className="grid gap-4 lg:grid-cols-[2fr_1fr]">
        <Card className="flex flex-col gap-4">
          <h2 className="text-xl font-extrabold">{t("mps.title")}</h2>
          <p className="rounded-md bg-elevated px-3 py-3 font-mono text-sm text-main">{t("mps.equation", { base: n(mps.baseScore) })}</p>
          <p className="text-sm text-secondary">{t("mps.clamp", { min: n(mps.minScore), max: n(mps.maxScore) })}</p>
          <p className="text-sm text-secondary">{t("mps.volumeNote")}</p>
        </Card>

        <Card className="flex flex-col gap-3">
          <h2 className="text-xl font-extrabold">{t("minutes.title")}</h2>
          <ul className="flex flex-col gap-2 text-sm text-secondary">
            <li>{t("minutes.partial", { full: minutes.fullFactorFromMinute, regulation: minutes.regulationMinutes })}</li>
            <li>{t("minutes.full", { full: minutes.fullFactorFromMinute, regulation: minutes.regulationMinutes })}</li>
            <li>{t("minutes.extraTime", { regulation: minutes.regulationMinutes, divisor: minutes.extraTimeDivisor })}</li>
            <li className="text-muted">{t("minutes.cameo", { min: minutes.minMinutesForSeason })}</li>
          </ul>
        </Card>
      </div>

      <section aria-labelledby="weights-title" className="flex flex-col gap-4">
        <header className="flex flex-col gap-1">
          <h2 id="weights-title" className="text-2xl font-black">
            {t("weights.title")}
          </h2>
          <p className="text-secondary">{t("weights.description")}</p>
        </header>
        <WeightMatrix actions={formula.actions} positions={formula.positions} version={formula.algorithmVersion} />
        <ul className="flex flex-col gap-1 text-xs text-muted">
          <li>∝ {t("weights.volumeLegend")}</li>
          <li>
            <span className="text-gold-glow">★</span> {t("weights.decisiveLegend")}
          </li>
          <li>{t("weights.noDataLegend")}</li>
          {sharedPositions.map((p) => (
            <li key={p.code}>{t("weights.sharedMatrix", { position: p.code, owner: p.sharesMatrixWith ?? "" })}</li>
          ))}
        </ul>
      </section>

      <section aria-labelledby="multipliers-title" className="flex flex-col gap-4">
        <header className="flex flex-col gap-1">
          <h2 id="multipliers-title" className="text-2xl font-black">
            {t("multipliers.title")}
          </h2>
          <p className="text-secondary">{t("multipliers.description")}</p>
        </header>
        <MultiplierCards formula={formula} />
      </section>

      <Card className="flex flex-col gap-4">
        <h2 className="text-xl font-extrabold">{t("fss.title")}</h2>
        <p className="rounded-md bg-elevated px-3 py-3 font-mono text-sm text-main">{t("fss.equation")}</p>
        <ul className="flex flex-col gap-2 text-sm text-secondary">
          <li>{t("fss.presence", { minutes: season.fullPresenceMinutes })}</li>
          <li>{t("fss.why")}</li>
          <li className="font-semibold text-main">
            {t("fss.eligibility", { matches: season.minMatchesForRanking, minutes: season.minMinutesForRanking })}
          </li>
        </ul>
      </Card>

      <Card highlighted className="flex flex-col items-start gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex flex-col gap-1">
          <h2 className="text-xl font-extrabold">{t("audit.title")}</h2>
          <p className="text-sm text-secondary">{t("audit.description")}</p>
        </div>
        <Link href="/ranking" className={buttonStyles("primary")}>
          {t("audit.cta")}
        </Link>
      </Card>
    </section>
  );
}
