import type { Locale } from "./routing";

import type ballon from "@/messages/pt-BR/ballon.json";
import type common from "@/messages/pt-BR/common.json";
import type formula from "@/messages/pt-BR/formula.json";
import type player from "@/messages/pt-BR/player.json";
import type positions from "@/messages/pt-BR/positions.json";
import type ranking from "@/messages/pt-BR/ranking.json";
import type receipt from "@/messages/pt-BR/receipt.json";

export const namespaces = ["common", "ranking", "player", "receipt", "formula", "ballon", "positions"] as const;

export type Namespace = (typeof namespaces)[number];

// pt-BR é o idioma de referência: as chaves dos demais locales devem espelhá-lo
export interface Messages {
  common: typeof common;
  ranking: typeof ranking;
  player: typeof player;
  receipt: typeof receipt;
  formula: typeof formula;
  ballon: typeof ballon;
  positions: typeof positions;
}

export async function loadMessages(locale: Locale): Promise<Messages> {
  const entries = await Promise.all(
    namespaces.map(async (namespace) => {
      const file = (await import(`../messages/${locale}/${namespace}.json`)) as { default: unknown };
      return [namespace, file.default] as const;
    }),
  );

  return Object.fromEntries(entries) as unknown as Messages;
}
