import Image from "next/image";

import { initials } from "@/lib/format";
import { cn } from "@/lib/utils";

interface PlayerAvatarProps {
  name: string;
  photoUrl: string | null;
  size?: number;
  highlighted?: boolean;
}

export function PlayerAvatar({ name, photoUrl, size = 40, highlighted = false }: PlayerAvatarProps) {
  const ring = highlighted ? "ring-2 ring-gold shadow-glow" : "ring-1 ring-line";

  if (!photoUrl) {
    return (
      <span
        aria-hidden
        style={{ width: size, height: size }}
        className={cn("flex shrink-0 items-center justify-center rounded-full bg-elevated text-xs font-bold text-secondary", ring)}
      >
        {initials(name)}
      </span>
    );
  }

  return (
    <Image
      src={photoUrl}
      alt=""
      width={size}
      height={size}
      className={cn("shrink-0 rounded-full bg-elevated object-cover", ring)}
    />
  );
}
