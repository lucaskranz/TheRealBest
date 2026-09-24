# The Real Best — Frontend

Next.js 16 (App Router) + TypeScript strict + Tailwind CSS 4 + next-intl 4.

## Rodando localmente

```bash
cp .env.example .env.local
npm install
npm run dev          # http://localhost:3000 → redireciona para /pt-BR, /en ou /es
```

## Scripts

| Script | Descrição |
|:---|:---|
| `npm run dev` | Servidor de desenvolvimento |
| `npm run build` | Build de produção (páginas localizadas pré-renderizadas) |
| `npm run lint` | ESLint (config do Next) |
| `npm run typecheck` | Gera os tipos de rota e roda `tsc --noEmit` |
| `npm run i18n:check` | Garante que `en` e `es` têm exatamente as mesmas chaves que `pt-BR` |

## Estrutura

```
src/
├── app/[locale]/        # Rotas localizadas (layout com <html lang>, páginas, not-found)
├── components/ui/       # Button, Card, Badge, Skeleton
├── components/layout/   # Header, Footer, NavLinks, LocaleSwitcher, BrandMark
├── i18n/                # routing, request (carregamento de mensagens), navigation
├── messages/{locale}/   # common, ranking, player, receipt, formula, positions
├── lib/                 # api (Accept-Language), seo (hreflang), fonts, utils
├── styles/design-tokens.css
└── proxy.ts             # Detecção de locale (no Next 16, middleware.ts virou proxy.ts)
```

## i18n

- Locales: `pt-BR`, `en`, `es`, sempre com prefixo na URL.
- Detecção: cookie `NEXT_LOCALE` > `Accept-Language` > fallback `pt-BR` (idioma padrão).
- `pt-BR` é o idioma de referência para os tipos: chamadas `t('...')` com chave inexistente não compilam.
- Novas strings: adicione a chave nos três locales e rode `npm run i18n:check`.

## Design system

Os tokens ficam em `src/styles/design-tokens.css` (variáveis `--trb-*`) e são expostos ao Tailwind em
`src/app/globals.css` via `@theme inline`: `bg-surface`, `bg-elevated`, `text-gold`, `text-secondary`,
`border-line`, `font-display`, `shadow-glow`, além da classe `.glass` (glassmorphism).
Fontes: Cabinet Grotesk (self-hosted em `src/fonts`, via `next/font/local`) e Inter (`next/font/google`).
