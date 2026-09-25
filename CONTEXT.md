# CONTEXT.md — The Real Best | Beyond The Ballon

> **Este documento é a fonte de verdade do projeto. Todo agente de IA, desenvolvedor ou contribuidor DEVE ler este arquivo antes de realizar qualquer trabalho.**

---

## 1. O Que É Este Projeto

**The Real Best** é uma plataforma web de ranking global de jogadores de futebol baseada exclusivamente em dados estatísticos jogo a jogo, eliminando o viés de fama, popularidade e política que distorce premiações tradicionais como a Bola de Ouro e o The Best da FIFA.

**Slogan:** *Beyond The Ballon. Beyond The Hype. Pure Performance.*

### Proposta de Valor
- Ranking 100% auditável: qualquer usuário pode clicar em qualquer jogador e ver o extrato matemático de cada ponto ganho ou perdido.
- Fórmulas posicionais justas: zagueiros, volantes, laterais, meias e atacantes são avaliados por métricas específicas de sua função tática.
- Fator anti-junk time: gols em goleadas contra times fracos valem menos que decisões em jogos equilibrados contra rivais de elite.
- Multilíngue: suporte completo para Português (BR), English e Español.

### Cobertura
- 5 principais ligas europeias (Premier League, La Liga, Serie A, Bundesliga, Ligue 1)
- UEFA Champions League
- Campeonatos de seleções (Copa do Mundo, Eurocopa, Copa América)

---

## 2. Stack Tecnológica

| Camada | Tecnologia | Justificativa |
|:---|:---|:---|
| **Backend** | ASP.NET Core 8 (C#) | Clean Architecture, Background Workers para ingestão, performance |
| **Frontend** | Next.js 14+ (React + TypeScript) | App Router, SSR para SEO, Server Components |
| **Banco de Dados** | PostgreSQL | Queries analíticas, JSONB para extratos auditáveis, tabelas de tradução |
| **Cache** | Redis | Cache de rankings (muda apenas após rodadas) |
| **ORM** | Entity Framework Core 8 | Code First, Migrations, LINQ |
| **i18n Frontend** | next-intl | Roteamento por locale, Server Components |
| **i18n Backend** | IStringLocalizer + .resx + DB | Labels estáticos em .resx, conteúdo dinâmico em tabelas |
| **Validação** | FluentValidation | Regras expressivas separadas dos controllers |
| **HTTP Client** | Refit + Polly | Clients tipados para APIs externas, retry |
| **Logging** | Serilog | Structured logging |
| **Testes** | xUnit + FluentAssertions + Moq | Unitários, integração |
| **Charts** | Recharts ou Nivo | Radar, line, bar charts |
| **CSS** | Tailwind CSS 4 | Design system via tokens |
| **Deploy** | Vercel (Front) + Railway/Render (Back+DB) | Gratuito para MVP |

---

## 3. Arquitetura do Backend (Clean Architecture)

### Projetos na Solution
`
TheRealBest.sln
├── src/
│   ├── TheRealBest.Domain           # Entidades, Value Objects, Interfaces, Enums
│   ├── TheRealBest.Application      # Use Cases, DTOs, Validators, Localization
│   ├── TheRealBest.Infrastructure   # EF Core, Repositories, APIs externas, Cache, Translations
│   ├── TheRealBest.Scoring          # Motor de pontuação isolado (Weights, Multipliers, Calculators)
│   └── TheRealBest.API              # Controllers, Middleware, Background Services, Resources (.resx)
├── tests/
│   ├── TheRealBest.Domain.Tests
│   ├── TheRealBest.Application.Tests
│   ├── TheRealBest.Scoring.Tests
│   └── TheRealBest.API.IntegrationTests
└── docs/
`

### Regras de Dependência (Clean Architecture)
- Domain → NENHUMA dependência externa
- Application → depende apenas de Domain
- Infrastructure → depende de Domain e Application
- Scoring → depende apenas de Domain
- API → depende de Application e Infrastructure (composição raiz)

---

## 4. Modelo de Dados (Tabelas Principais)

| Tabela | Propósito |
|:---|:---|
| players | Cadastro de jogadores (nome, nacionalidade, posição primária, foto) |
| teams | Clubes (nome, logo, país, ELO ranking) |
| competitions | Torneios (nome, tier, multiplicador, temporada) |
| matches | Partidas (times, placar, competição, data, fase) |
| match_player_stats | Estatísticas brutas por jogador por partida (todas as métricas da API) |
| match_performance_scores | MPS calculado com extrato JSONB auditável (action_breakdown, penalty_breakdown) |
| season_rankings | FSS acumulado, rank geral, rank por posição, clutch index |
| scoring_weights | Pesos configuráveis por posição e ação (versionados) |
| competition_translations | Traduções de nomes de competições por locale |
| action_type_translations | Reservada (sem uso): os rótulos das ações vêm de `ActionLabels.resx` |

---

## 5. O Algoritmo de Pontuação (Fair Player Index)

### Fórmula Central (MPS - Match Performance Score)
`
MPS = Clamp(0, 100, Base + (ΔAções − LinhaDeBase_posição × FatorMinutos) × MultContexto)
`

- **Base = 50.0** (nota neutra: uma atuação média vale 50 em qualquer posição e contexto)
- **ΔAções:** Soma de eventos positivos e negativos com pesos específicos por posição
- **LinhaDeBase_posição:** ΔAções médio de uma atuação completa na posição, medido em dados reais (algoritmo v2)
- **FatorMinutos:** Normalização (< 60min = M/90; 60-90min = 1.0; prorrogação = bônus), aplicada só às ações de volume
- **MultContexto = W_torneio × W_adversário × W_clutch**

### Ranking da Temporada (FSS - Fair Season Score)
`
FSS = (Σ MPS_i × W_torneio_i / Σ W_torneio_i) × FatorPresença
Elegível para o ranking: mínimo de 10 partidas E 900 minutos na temporada
FatorPresença = min(1.0, MinutosJogados / 2200)^0.5
`

> A especificação completa com todas as matrizes de peso por posição está em docs/fair_ranking_formula_specification.md. A seção 7 dela descreve o algoritmo implementado (v1).

---

## 6. Internacionalização (i18n)

| Locale | Idioma | URL Pattern | Status |
|:---:|:---|:---|:---:|
| pt-BR | Português (Brasil) | /pt-BR/* | **Padrão** |
| en | English | /en/* | Suportado |
| es | Español | /es/* | Suportado |

- Frontend: 
ext-intl com roteamento por [locale] e detecção automática via cookie/Accept-Language
- Backend: `LocalizationMiddleware` lê o Accept-Language (pesos q, idioma principal: pt-PT → pt-BR, en-US → en) e responde com `Content-Language`; textos estáticos (rótulos do recibo, posições, mensagens) em `.resx` (`src/TheRealBest.API/Resources`); nomes de competição em `competition_translations`, preenchida na ingestão
- Nomes de jogadores e times NÃO são traduzidos

---

## 7. Fontes de Dados Externas

| Fonte | Uso | Prioridade |
|:---|:---|:---:|
| **API-Football (RapidAPI)** | Estatísticas brutas por partida (85% das métricas) | Principal |
| **FBref** | Dados avançados (xG, xA, Big Chances, SCA) | Enriquecimento |
| **Sofascore** | Validação cruzada e dados complementares | Futuro |

### API-Football: acesso e limites
- Acesso direto pela api-sports.io (`https://v3.football.api-sports.io`, header `x-apisports-key`), cliente em `Infrastructure/ExternalApis/ApiFootball`
- A chave **nunca** vai para o repositório: `dotnet user-secrets set "ApiFootball:ApiKey" "<chave>" --project src/TheRealBest.API`
- **Plano Free:** só temporadas 2022–2024, 100 requisições/dia e 10/min. **Pro:** todas as temporadas, 7.500/dia e 300/min, sem renovação automática. Ajustar `ApiFootball:RequestsPerMinute` ao plano
- Exceder o limite por minuto pode bloquear a conta: o cliente enfileira e espaça as requisições e para quando a cota diária acaba (zera às 00:00 UTC)
- Custo: 1 requisição por listagem de partidas; 3 por partida importada (jogadores, eventos, escalações)
- A API-Football **não** fornece xG, xA, grandes chances, recuperações, passes progressivos nem duelos aéreos separados: esses campos ficam zerados até o enriquecimento via FBref
- **Cache de respostas** (`ApiFootball:ResponseCacheEnabled`, ligado em Development): respostas de partidas encerradas ficam em `%LOCALAPPDATA%/TheRealBest/api-football-cache`, fora do repositório. Recriar o banco ou repetir o seed não gasta cota
- O parâmetro `ids` (várias partidas por requisição, com jogadores e eventos embutidos) **não** existe no plano Free; num plano pago, reduz muito o custo de carga

### Seed com dados reais
- **Nunca** usar dados digitados à mão: todo dado vem da API-Football, identificado pelo ID externo da fonte
- `dotnet run --project src/TheRealBest.API -- --seed` importa as partidas de `Infrastructure/Data/Seeds/RealMatchSelection.cs` (30 jogos do ciclo da Bola de Ouro 2024) e recalcula o ranking. Não roda automaticamente ao subir a API
- Retomável: partidas já importadas são puladas, e se a cota diária acabar basta rodar de novo após 00:00 UTC
- Euro e Copa América de junho/julho entram na temporada de clubes que terminou (Euro 2024 → temporada 2023/24)
- Após reimportar dados, o frontend pode mostrar a versão anterior por até 1 hora (cache de dados do Next, que sobrevive a novos builds). Para ver na hora: apague `frontend/.next/cache/fetch-cache`
- **Comparação com a Bola de Ouro:** a classificação oficial fica em `Application/Comparison/BallonDorCatalog.cs`, com fonte citada e o ID externo de cada indicado. Indicados sem partidas importadas aparecem como "dados insuficientes" e os que não atingem o corte aparecem como "abaixo do corte". Nunca estimar nota
- **Página do algoritmo:** nenhum número digitado no frontend. `FormulaDescriptorFactory` (API) lê os pesos, as linhas de base e os multiplicadores do motor e marca as ações que a fonte atual não alimenta (`ApiFootballMapper.UnavailableActions`, `StatsActionMapper.UnmappedActions`)
- **Elo dos adversários:** buscado no ClubElo (`api.clubelo.com`, gratuito) na data de cada partida e gravado em `matches`. Seleções e falhas da fonte = multiplicador neutro. Partidas importadas sem Elo não são reprocessadas: para preenchê-lo, limpe as tabelas e rode `--seed` de novo (custo zero de cota graças ao cache)

---

## 8. Roadmap de Implementação por Etapas

O projeto está dividido em **etapas atômicas** que podem ser executadas independentemente em conversas separadas com o agente de IA. Cada etapa produz um resultado funcional e commitável.

### Status das Etapas

| # | Etapa | Status | Descrição |
|:---:|:---|:---:|:---|
| 1A | Solution .NET + Estrutura Clean Architecture | ✅ Concluída | Criar solution, projetos, referências |
| 1B | Entidades de Domínio e Enums | ✅ Concluída | Player, Team, Match, MatchPlayerStats, etc. |
| 1C | Banco de Dados (EF Core + Migrations) | ✅ Concluída | DbContext, Configurations, Migration inicial |
| 1D | Frontend Next.js + i18n + Design System | ✅ Concluída | Setup Next.js 14, next-intl, Tailwind, tokens |
| 2A | Motor de Pontuação (Scoring Engine) | ✅ Concluída | Weight matrices, calculators, multipliers |
| 2B | Testes do Motor de Pontuação | ✅ Concluída | Cenários reais (Rodri, Vinicius Jr, etc.) |
| 3A | Client API-Football + Mapper | ✅ Concluída | HttpClient tipado, modelos, mapeamento |
| 3B | Background Workers de Ingestão | ✅ Concluída | MatchDataIngestionService, pipeline |
| 3C | Seeds com Dados Reais | ✅ Concluída | 30 partidas reais do ciclo 2023/24 via API-Football (`--seed`), algoritmo v2 calibrado; pendente: Elo real (ClubElo fora do ar na implementação) |
| 4A | Controllers REST (Ranking + Players) | ✅ Concluída | Endpoints, DTOs, paginação, filtros |
| 4B | Controllers REST (Audit + Matches) | ✅ Concluída | Recibo auditável, detalhamento de partida |
| 4C | Localização no Backend | ✅ Concluída | Middleware, .resx, ActionLabelResolver |
| 4D | Frontend: Leaderboard + Filtros | ✅ Concluída | Página de ranking com filtros posicionais |
| 4E | Frontend: Perfil do Jogador + Recibo | ✅ Concluída | Player page, MatchReceipt, radar chart |
| 4F | Frontend: Vs Ballon d'Or + Fórmula | ✅ Concluída | `/formula` (pesos, linhas de base e multiplicadores via `GET /api/v1/formula`, gerado das constantes do motor) e `/vs-ballon` (classificação oficial 2024 citada × FSS 2023/24 via `GET /api/v1/comparison/ballon-dor/2024`) |
| 5A | Deploy Backend (Railway/Render) | ⬜ Pendente | Docker, CI/CD, variáveis de ambiente |
| 5B | Deploy Frontend (Vercel) | ⬜ Pendente | Build, domínio, SEO multilíngue |

### Como Usar Este Roadmap
1. Ao iniciar uma nova conversa, o agente DEVE ler este CONTEXT.md
2. Identificar a próxima etapa com status ⬜ Pendente
3. Ao finalizar a etapa, atualizar o status para ✅ Concluída
4. Commitar as alterações com mensagem descritiva

---

## 9. Convenções do Projeto

### Commits
- Formato: 	ipo(escopo): descrição
- Tipos: eat, ix, docs, 
efactor, 	est, chore, style
- Exemplos: eat(scoring): add weight matrix for center backs, eat(i18n): add Spanish translations

### Branches
- main — código estável e deployável
- eature/etapa-XX-descricao — branch por etapa do roadmap
- Merge via PR (ou direto em main durante o MVP)

### Código
- Backend: C# com nullable reference types habilitado, PascalCase
- Frontend: TypeScript strict mode, camelCase para variáveis/funções, PascalCase para componentes
- Todos os textos visíveis ao usuário DEVEM usar o sistema de tradução (nunca strings hardcoded)

