import { useTranslations } from "next-intl";
import type { ReactNode } from "react";

import { Card } from "@/components/ui/Card";

const iconProps = {
  viewBox: "0 0 24 24",
  fill: "none",
  stroke: "currentColor",
  strokeWidth: 2,
  strokeLinecap: "round",
  strokeLinejoin: "round",
  className: "size-6",
  "aria-hidden": true,
} as const;

const pillars: Array<{ key: "auditable" | "positional" | "antiJunkTime"; icon: ReactNode }> = [
  {
    key: "auditable",
    icon: (
      <svg {...iconProps}>
        <path d="M9 11l3 3L22 4" />
        <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11" />
      </svg>
    ),
  },
  {
    key: "positional",
    icon: (
      <svg {...iconProps}>
        <rect x="3" y="3" width="18" height="18" rx="2" />
        <line x1="12" y1="3" x2="12" y2="21" />
        <circle cx="12" cy="12" r="3" />
      </svg>
    ),
  },
  {
    key: "antiJunkTime",
    icon: (
      <svg {...iconProps}>
        <path d="M12 3v18" />
        <path d="M5 7h14" />
        <path d="M5 7l-3 7a4 4 0 0 0 6 0z" />
        <path d="M19 7l-3 7a4 4 0 0 0 6 0z" />
      </svg>
    ),
  },
];

export function Pillars() {
  const t = useTranslations("common.home.pillars");

  return (
    <section aria-labelledby="pillars-title" className="mx-auto max-w-6xl px-4 py-12 sm:px-6">
      <h2 id="pillars-title" className="mb-8 text-2xl font-extrabold sm:text-3xl">
        {t("title")}
      </h2>

      <div className="grid gap-4 md:grid-cols-3">
        {pillars.map(({ key, icon }) => (
          <Card key={key} interactive className="flex flex-col gap-4">
            <span className="flex size-12 items-center justify-center rounded-md bg-gold-subtle text-gold">
              {icon}
            </span>
            <h3 className="text-xl font-bold">{t(`${key}.title`)}</h3>
            <p className="text-secondary">{t(`${key}.description`)}</p>
          </Card>
        ))}
      </div>
    </section>
  );
}
