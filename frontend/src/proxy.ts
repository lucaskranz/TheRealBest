import createMiddleware from "next-intl/middleware";

import { routing } from "./i18n/routing";

// Detecção de locale: cookie NEXT_LOCALE > Accept-Language > defaultLocale (en)
export default createMiddleware(routing);

export const config = {
  // Ignora rotas de API, internas do Next/Vercel e arquivos estáticos (qualquer caminho com ponto)
  matcher: "/((?!api|_next|_vercel|.*\\..*).*)",
};
