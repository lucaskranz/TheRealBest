/** Temporada europeia a partir do ano de início: 2023 → "2023/24". */
export function formatSeason(startYear: number): string {
  return `${startYear}/${String((startYear + 1) % 100).padStart(2, "0")}`;
}

/** Iniciais para o avatar quando o jogador não tem foto: "Vinícius Júnior" → "VJ". */
export function initials(name: string): string {
  const parts = name.trim().split(/\s+/);
  return (parts.length === 1 ? parts[0].slice(0, 2) : parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
}
