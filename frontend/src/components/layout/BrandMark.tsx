import { useTranslations } from "next-intl";

import { Link } from "@/i18n/navigation";

export function BrandMark() {
  const t = useTranslations("common.brand");

  return (
    <Link href="/" aria-label={t("homeLabel")} className="group flex items-center gap-3">
      <span className="flex size-10 items-center justify-center rounded-sm bg-gradient-to-br from-gold to-gold-deep text-on-gold shadow-glow transition-transform duration-[var(--trb-duration)] ease-out-soft group-hover:scale-105">
        <svg
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2.2"
          strokeLinecap="round"
          strokeLinejoin="round"
          className="size-5"
          aria-hidden
        >
          <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2" />
        </svg>
      </span>
      <span className="flex flex-col leading-tight">
        <span className="font-display text-lg font-black tracking-wide uppercase">{t("name")}</span>
        <span className="text-[0.65rem] font-semibold tracking-[0.2em] text-gold uppercase">{t("tagline")}</span>
      </span>
    </Link>
  );
}
