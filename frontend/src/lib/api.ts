import type { Locale } from "@/i18n/routing";

import type { ApiResponse } from "./types";

// Server Components chamam a API direto; API_URL permite um endereço interno diferente do público
const apiBaseUrl = process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5127";

/** O ranking só muda quando novas partidas são importadas: 1 hora de cache é seguro. */
export const DEFAULT_REVALIDATE_SECONDS = 3600;

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly path: string,
  ) {
    super(`API request to ${path} failed with status ${status}`);
    this.name = "ApiError";
  }
}

interface ApiGetOptions {
  locale: Locale;
  query?: Record<string, string | number | boolean | undefined>;
  revalidate?: number;
}

/**
 * GET na API .NET. Sempre envia Accept-Language com o locale atual, para que o backend devolva
 * rótulos (recibo, posições, competições) e mensagens já traduzidos.
 */
export async function apiGet<T>(path: string, { locale, query, revalidate = DEFAULT_REVALIDATE_SECONDS }: ApiGetOptions) {
  const url = new URL(`/api/v1${path}`, apiBaseUrl);
  for (const [key, value] of Object.entries(query ?? {})) {
    if (value !== undefined && value !== "") {
      url.searchParams.set(key, String(value));
    }
  }

  const response = await fetch(url, {
    headers: { Accept: "application/json", "Accept-Language": locale },
    next: { revalidate },
  });

  if (!response.ok) {
    throw new ApiError(response.status, path);
  }

  return (await response.json()) as ApiResponse<T>;
}
