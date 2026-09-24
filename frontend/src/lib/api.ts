import type { Locale } from "@/i18n/routing";

const apiBaseUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5127";

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly path: string,
  ) {
    super(`API request to ${path} failed with status ${status}`);
    this.name = "ApiError";
  }
}

interface ApiFetchOptions extends RequestInit {
  locale: Locale;
}

/**
 * Client HTTP para a API .NET. Sempre envia Accept-Language com o locale atual,
 * para que o backend devolva labels (recibo, posições, competições) já traduzidos.
 */
export async function apiFetch<T>(path: string, { locale, headers, ...init }: ApiFetchOptions): Promise<T> {
  const response = await fetch(`${apiBaseUrl}/api/v1${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      "Accept-Language": locale,
      ...headers,
    },
  });

  if (!response.ok) {
    throw new ApiError(response.status, path);
  }

  return (await response.json()) as T;
}
