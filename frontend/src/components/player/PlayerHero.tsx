import { useTranslations } from "next-intl";
import Image from "next/image";

import { PlayerAvatar } from "@/components/ranking/PlayerAvatar";
import { Badge } from "@/components/ui/Badge";
import { Link } from "@/i18n/navigation";
import type { PlayerProfile } from "@/lib/types";

interface PlayerHeroProps {
  profile: PlayerProfile;
}

/** A fonte ainda não informa nacionalidade (a ingestão grava "Unknown"): nesse caso ela é omitida. */
const UNKNOWN_NATIONALITY = "Unknown";

export function PlayerHero({ profile }: PlayerHeroProps) {
  const t = useTranslations("player");
  const positions = useTranslations("positions");
  const ranking = profile.seasonRanking;

  return (
    <header className="flex flex-col gap-6">
      <Link href="/ranking" className="text-sm font-semibold text-secondary transition-colors hover:text-gold-glow">
        ← {t("backToRanking")}
      </Link>

      <div className="flex flex-col items-start gap-5 sm:flex-row sm:items-center">
        <PlayerAvatar name={profile.name} photoUrl={profile.photoUrl} size={96} highlighted={ranking?.overallRank === 1} />

        <div className="flex flex-col gap-3">
          <h1 className="text-4xl leading-none font-black sm:text-5xl">{profile.name}</h1>
          <div className="flex flex-wrap items-center gap-3 text-sm text-secondary">
            <Badge tone="gold" title={profile.primaryPositionLabel}>
              {positions(`names.${profile.primaryPosition}`)}
            </Badge>
            {ranking?.teamName && (
              <span className="flex items-center gap-2">
                {ranking.teamLogoUrl && <Image src={ranking.teamLogoUrl} alt="" width={20} height={20} />}
                {ranking.teamName}
              </span>
            )}
            {profile.nationality !== UNKNOWN_NATIONALITY && <span>{profile.nationality}</span>}
          </div>
        </div>
      </div>
    </header>
  );
}
