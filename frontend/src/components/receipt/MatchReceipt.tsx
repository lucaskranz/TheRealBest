import { useFormatter, useTranslations } from "next-intl";

import { PlayerAvatar } from "@/components/ranking/PlayerAvatar";
import { Card } from "@/components/ui/Card";
import { Link } from "@/i18n/navigation";
import { matchTitle } from "@/lib/format";
import type { MatchReceipt as MatchReceiptData } from "@/lib/types";

import { ReceiptCalculation } from "./ReceiptCalculation";
import { ReceiptItems } from "./ReceiptItems";

interface MatchReceiptProps {
  receipt: MatchReceiptData;
}

/** O recibo auditável: renderiza exatamente o que o backend gravou, no idioma da requisição. */
export function MatchReceipt({ receipt }: MatchReceiptProps) {
  const t = useTranslations("receipt");
  const format = useFormatter();
  const title = matchTitle(receipt.homeTeamName, receipt.awayTeamName, receipt.homeScore, receipt.awayScore);

  return (
    <article className="flex flex-col gap-6">
      <header className="flex flex-col gap-5">
        <Link
          href={`/player/${receipt.playerId}`}
          className="text-sm font-semibold text-secondary transition-colors hover:text-gold-glow"
        >
          ← {t("backToPlayer", { player: receipt.playerName })}
        </Link>
        <div className="flex items-center gap-4">
          <PlayerAvatar name={receipt.playerName} photoUrl={receipt.photoUrl} size={56} />
          <div className="flex flex-col gap-1">
            <p className="text-sm font-semibold tracking-wide text-gold uppercase">{t("title")}</p>
            <h1 className="text-2xl leading-tight font-black sm:text-3xl">{title}</h1>
            <p className="text-sm text-secondary">
              {receipt.playerName} · {receipt.competitionName} ·{" "}
              {format.dateTime(new Date(receipt.matchDate), { day: "2-digit", month: "long", year: "numeric" })}
            </p>
            <p className="text-sm text-muted">
              {t("details.minutes", { minutes: receipt.minutesPlayed })} ·{" "}
              {t("details.position", { position: receipt.playerPositionLabel })}
            </p>
          </div>
        </div>
      </header>

      <div className="grid gap-6 lg:grid-cols-[1fr_22rem]">
        <Card className="flex flex-col gap-8">
          <ReceiptItems title={t("actionsTitle")} emptyText={t("noActions")} items={receipt.positiveActions} />
          <ReceiptItems title={t("penaltiesTitle")} emptyText={t("noPenalties")} items={receipt.penalties} />
        </Card>
        <ReceiptCalculation
          formula={receipt.formula}
          competitionName={receipt.competitionName}
          algorithmVersion={receipt.algorithmVersion}
        />
      </div>
    </article>
  );
}
