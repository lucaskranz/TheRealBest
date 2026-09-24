# Rules: Frontend (Next.js / React / TypeScript)

## Estrutura e Arquitetura
- SEMPRE usar App Router (Next.js 14+) com o pattern [locale] para i18n
- Server Components por padrão; usar 'use client' apenas quando necessário (interatividade, hooks)
- Componentes organizados por domínio: ui/, layout/, ranking/, player/, comparison/
- Componentes devem ser focados e reutilizáveis (max ~150 linhas)

## TypeScript
- Strict mode obrigatório (strict: true no tsconfig)
- Interfaces para props de componentes (não types, exceto unions)
- Nunca usar ny; preferir unknown quando o tipo é incerto
- Tipagens que espelham DTOs do backend devem ficar em lib/types.ts

## Internacionalização (i18n)
- Biblioteca: next-intl (NUNCA i18next ou react-intl)
- Todas as strings visíveis ao usuário DEVEM usar useTranslations() ou getTranslations()
- NUNCA hardcodar textos em português, inglês ou espanhol nos componentes
- Arquivos de tradução organizados por namespace: common, ranking, player, receipt, formula, positions
- Cada namespace é um JSON separado por locale em src/messages/{locale}/
- O src/proxy.ts (Next 16: substitui o antigo middleware.ts) detecta o locale (cookie > Accept-Language > fallback pt-BR, o idioma padrão)
- LocaleSwitcher no header permite troca manual e salva preferência em cookie NEXT_LOCALE
- Toda página deve gerar meta tags hreflang para SEO multilíngue

## Estilização
- Tailwind CSS 4 como framework principal
- Design tokens definidos em CSS custom properties (src/styles/design-tokens.css), expostos ao Tailwind via @theme inline em globals.css
- Dark mode como padrão; paleta: grafite profundo, dourado acetinado, verde esmeralda
- Glassmorphism com backdrop-filter para cards elevados
- Tipografia: Cabinet Grotesk (display, self-hosted via next/font/local — não existe no Google Fonts) + Inter (body, via next/font/google)
- Micro-animações suaves (transitions, hover effects) em elementos interativos
- Responsividade obrigatória (mobile-first approach)

## Componentes Chave
- MatchReceipt: componente central que renderiza o extrato auditável de pontuação
- LeaderboardTable: tabela de ranking com ordenação, filtros posicionais e busca
- PlayerHero: header do perfil do jogador com avatar, stats e posição
- RadarChart: gráfico de teia comparativo de atributos
- LocaleSwitcher: seletor de idioma com bandeiras

## Data Fetching
- Server Components fazem fetch direto da API .NET (sem hooks)
- Client Components usam hooks customizados (useRanking, usePlayerProfile)
- O client HTTP (lib/api.ts) DEVE enviar o header Accept-Language com o locale atual
- ISR (Incremental Static Regeneration) para páginas de ranking (revalidate a cada rodada)
- Server Components buscam a API com `apiGet` (lib/api.ts), que envia Accept-Language e usa cache de 1 hora (`next.revalidate`)
- Filtros e paginação ficam na URL (?position=CDM&search=rod&page=2): links e `next/form`, sem estado no cliente; valores inválidos são ignorados, nunca quebram a página
- Toda página que depende da API trata falha com mensagem traduzida (a API fora do ar não pode derrubar a página)
- Rotas de jogador usam o ID da API .NET: /[locale]/player/[id] (Etapa 4E)
- Imagens externas (fotos e escudos) só de media.api-sports.io, liberado em next.config.ts (images.remotePatterns)

## SEO
- Cada página deve ter metadata dinâmico (generateMetadata) com título e descrição localizados
- Open Graph tags com imagens e descrições por idioma
- Sitemap multilíngue gerado automaticamente
- Um único h1 por página, hierarquia semântica de headings

## Performance
- Lazy loading para charts e componentes pesados
- Image optimization via next/image
- Target: Lighthouse 90+ em todas as métricas
