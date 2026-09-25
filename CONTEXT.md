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
- **Importação de temporada (etapa 5A):** `dotnet run --project src/TheRealBest.API -- --import-season 2023 [--competition 9] [--limit 5]` importa as partidas encerradas do escopo da temporada (`Infrastructure/ExternalApis/ApiFootball/ApiFootballSeasonScope.cs`), pula as já gravadas, para sem falhar quando a cota acaba e recalcula o ranking. Termina com um relatório por competição (listadas, encerradas, já no banco, importadas, sem dados de jogador, pendentes). `--limit` serve para testes no plano Free
- **Plano Pro:** ligar `ApiFootball:BatchFixtureDetails` (1 requisição a cada 20 partidas, com jogadores, eventos e escalações embutidos) e subir `ApiFootball:RequestsPerMinute` para 300, via user-secrets ou appsettings. O formato da resposta em lote foi testado com respostas gravadas dos endpoints separados; conferir a primeira execução real no Pro
- **Conferência (etapa 5B):** `--audit-season 2023 [--competition 39]` compara, por competição, as partidas encerradas na API com as gravadas: faltando, sem dados de jogador, sem posição tática (escalação sem grid: todos em GK/CB/CM/ST) e sem Elo, e lista os clubes sem rating. Custo zero nas temporadas encerradas (listagem no cache); termina com código 2 se algo estiver faltando
- **Elo depois da importação:** `--backfill-elo 2023` preenche o Elo das partidas de clubes gravadas sem ele (ClubElo fora do ar ou nome sem apelido em `ClubEloNames`), recalcula as notas dessas partidas a partir das estatísticas gravadas e o ranking. Não gasta cota da API-Football; pode ser repetido até a conferência não mostrar partidas sem Elo
- **Gols anulados pelo VAR:** a fonte normalmente não lista o gol anulado como evento "Goal". O `MatchTimeline` só aplica o "Goal cancelled" quando os gols listados excedem o placar final; antes, apagava um gol válido do mesmo time e zerava gols sofridos de quem estava em campo
- **Gravação atômica:** cada partida (partida + estatísticas + notas) é gravada num único `SaveChanges`; uma execução interrompida não deixa partida pela metade. Partidas sem estatísticas de jogador na fonte ficam gravadas sem jogadores e não são buscadas de novo
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
| 5A | Ingestão de temporadas completas | ✅ Concluída | `--import-season <ano> [--competition <id>] [--limit <n>]`, escopo por temporada em `ApiFootballSeasonScope`, lotes de 20 via `ids` com `ApiFootball:BatchFixtureDetails`, gravação atômica por partida; validado no Free com a Copa América 2024 |
| 5B | Carga histórica 2022/23 → 2025/26 | ✅ Concluída | 9.258 partidas nas 4 temporadas, conferidas com `--audit-season` (0 faltando). Copas só com partidas de times da primeira divisão. **Elo pendente:** a API do ClubElo foi fechada (autenticação, cadastro fechado desde ~09/2026); partidas de clubes com adversário neutro até existir uma fonte (Elo próprio a decidir) e rodar `--backfill-elo` |
| 5C | Elo próprio + eliminatórias fora | ⬜ Pendente | Elo calculado com os resultados gravados (substitui o ClubElo, fechado), aquecimento com 2022/23, `--backfill-elo` nas 4 temporadas; eliminatórias da Champions deixam de contar; Copa do Mundo de Clubes 2025 entra em 2024/25. **Próxima etapa** |
| 5D | Top 10 ligas do mundo (Opta) | ⬜ Pendente | As 10 ligas mais fortes pelo Opta Power Rankings, com copa nacional e continental de cada país e peso de torneio derivado do rating da fonte citada. **Fazer antes da 5E** |
| 5E | Recalibração do algoritmo (v3) | ⬜ Pendente | Linhas de base empíricas com as temporadas completas, revisão de posições e recálculo em massa |
| 5F | Ranking Top 50 multi-temporada | ⬜ Pendente | Seletor de temporada, Top 50 por temporada, histórico de temporadas no perfil do jogador |
| 5G | Temporada atual (2026/27) ao vivo | ⬜ Pendente | Atualização incremental semanal, ranking provisório com corte proporcional, variação desde a última rodada |
| 5H | Vs. Bola de Ouro multi-edição | ⬜ Pendente | Edições 2023, 2024 e 2025 (e 2026 quando publicada), sempre com a classificação oficial citada |
| 5I | Melhores de cada campeonato | ⬜ Pendente | Ranking por competição e temporada (ex.: Premier League 23/24, Copa do Mundo 2026), com corte e fator de presença proporcionais à competição |
| 6A | Deploy Backend / Dados | ⬜ Pendente | Hospedagem gratuita; avaliar site estático gerado localmente vs. API no ar |
| 6B | Deploy Frontend + atualização automática | ⬜ Pendente | Build, domínio, SEO multilíngue, rotina semanal (ex.: GitHub Actions) para a temporada atual |

### Fase 5 — Dados em escala (plano Pro da API-Football)

Decisão do usuário: assinar o plano **Pro** (7.500 requisições/dia, 300/min) para ter o histórico completo e acompanhar a temporada atual. O seed de 30 partidas (`--seed`) continua como demonstração no plano Free.

**Temporadas:** 2022/23, 2023/24, 2024/25, 2025/26 (históricas, completas) e 2026/27 (atual, em andamento).

**Escopo por temporada** (todas as partidas, não só as de candidatos):

| Temporada | Clubes | Seleções |
|:---|:---|:---|
| 2022/23 | 5 grandes ligas, Champions League, copas nacionais (FA Cup, Copa del Rey, Coppa Italia, DFB-Pokal, Coupe de France) | Copa do Mundo 2022 |
| 2023/24 | idem | Euro 2024, Copa América 2024 |
| 2024/25 | idem (Champions no novo formato) | — |
| 2025/26 | idem | Copa do Mundo 2026 |
| 2026/27 | idem, em andamento | — |

**Estimativa de custo:** ~2.000–2.400 partidas por temporada. Sem `ids` (3 requisições por partida) ≈ 6.600 req/temporada, cerca de 1 dia de cota cada. Com `ids` (20 partidas por requisição, com jogadores, eventos e escalações embutidos) ≈ 150–350 req/temporada: o histórico inteiro cabe em um dia. A temporada atual custa < 250 req/semana.

**Detalhamento das etapas:**
- **5A:** comando `--import-season <ano> [--competition <id>]` que lista as partidas de cada competição (1 requisição por competição/temporada) e importa só as encerradas e ainda não gravadas, em lotes via `ids` quando o plano permitir (fallback de 3 requisições por partida no Free). Retomável, respeita a cota diária e o limite por minuto (`ApiFootball:RequestsPerMinute` = 300 no Pro), usa o cache em disco. Validar antes da assinatura com uma competição pequena no Free (ex.: Copa América 2024).
- **5B:** rodar a carga das 4 temporadas históricas. Relatório de conferência por competição (partidas esperadas × importadas, partidas sem estatísticas de jogador, jogadores sem posição). Elo do ClubElo por data de jogo.
- **5E:** recalcular as linhas de base por posição com as temporadas completas (nova versão do algoritmo, v3; v1/v2 continuam reproduzíveis). Revisar a resolução de posição (ex.: volantes escalados como zagueiros em parte da temporada). Recalcular MPS/FSS em lote com desempenho aceitável (dezenas de milhares de atuações). Reavaliar o corte de elegibilidade (hoje 10 partidas e 900 minutos) **com o usuário** antes de mudar.
- **5F:** o ranking exibe o **Top 50** da temporada (o filtro por posição mostra o Top 50 da posição). A busca continua encontrando qualquer jogador elegível, e o perfil mostra a colocação mesmo fora do Top 50. Seletor de temporada (22/23 → 26/27) no ranking e no perfil; perfil com a evolução do jogador entre temporadas.
- **5G:** atualização incremental (partidas encerradas desde a última execução) e recálculo do ranking. Enquanto poucos jogadores atingem o corte, mostrar um **ranking provisório** com corte proporcional às rodadas disputadas, identificado como tal. Exibir "atualizado em" e a variação de posição desde a atualização anterior. Reaproveitar o `SeasonImporter` no worker em background: o `MatchDataIngestionPipeline` atual recalcula com a temporada da API, errada para torneios de seleções.
- **5H:** catálogos oficiais da Bola de Ouro 2023 (temporada 22/23), 2025 (24/25) e 2026 (25/26, só depois de publicada), com fonte citada como na edição 2024. Seletor de edição na página `/vs-ballon`.
- **5C (decisões do usuário, 25/09/2026):**
  - **Eliminatórias da Champions não contam** (fases preliminares e qualificatórias com clubes de ligas pequenas): ficam fora do escopo da importação e da conferência, como as fases amadoras das copas
  - **Elo próprio** no lugar do ClubElo (API fechada desde ~09/2026): partidas em ordem cronológica; cada uma grava o Elo dos dois times **antes** do jogo (sem informação do futuro); mando de campo e saldo de gols como no ClubElo. As ligas se calibram entre si pelos jogos de Champions e copas
  - **Aquecimento:** processar 2022/23 uma vez só para aquecer e usar as notas finais como ponto de partida do cálculo definitivo. Promovidos entram abaixo da média da liga; clubes de fora da primeira divisão recebem uma nota fixa documentada
  - **Auditável:** a fórmula e as constantes vão para `/formula`, e o recibo mostra o Elo usado e a origem ("Elo próprio"). Depois rodar `--backfill-elo` nas 4 temporadas e conferir com `--audit-season`
  - O mesmo cálculo serve para as ligas de fora da Europa da 5D
  - **Copa do Mundo de Clubes da FIFA 2025** (EUA, 14/06–13/07/2025, 32 clubes, 63 jogos; conferir o ID na API-Football): entra no escopo de **2024/25** (regra dos torneios de junho e julho). Novo tier de clubes com **peso por fase**: grupos 1,10 e mata-mata 1,35, como a Champions. Tem Elo (é de clubes), e os jogos entre continentes ajudam a calibrar o Elo próprio. Custo ≈ 4 requisições. Clubes de fora da elite europeia com poucos jogos recebem a nota fixa documentada
- **5D:** ampliar a cobertura para as **10 ligas mais fortes do mundo segundo o Opta Power Rankings** (decisão do usuário: fonte Opta; entram liga, copa nacional e continental de cada país novo, como nas 5 grandes). Regras:
  - **Fonte citada:** os ratings médios das ligas ficam num catálogo (como o `BallonDorCatalog`), com data da edição e URL. Nunca estimar: sem valor publicado, a liga não entra. A página `/formula` mostra o peso e a fonte
  - **Peso do torneio** derivado do rating médio da liga (proposta a validar **com o usuário**: proporcional ao rating relativo à Premier League, que fica em 1,10). Vale também para as 5 grandes, que hoje têm o mesmo peso: por isso entra junto com o algoritmo v3 (5E), e v1/v2 continuam reproduzíveis
  - **Continentais sul-americanos** (Libertadores, Sul-Americana) em tiers próprios. Hoje `InternationalContinental` é o tier de Euro e Copa América e é tratado como torneio de seleções (sem Elo)
  - **Calendário de ano civil** (Brasil, Argentina e outras): buscar as duas edições que tocam a temporada europeia e distribuir as partidas por data (julho a junho). O `SeasonImporter` hoje associa uma edição inteira a uma temporada
  - **Força do adversário:** o ClubElo só cobre clubes europeus. Para os demais, um Elo próprio calculado a partir dos resultados gravados, identificado como tal no recibo. Sem isso, o fator anti-junk time fica neutro nessas ligas
  - **Cobertura da fonte:** antes de incluir, conferir no `/leagues` da API-Football (campo `coverage`) se a liga tem estatísticas de jogador em todas as temporadas. Ligas ou temporadas sem esses dados ficam fora e aparecem no relatório `--audit-season`
  - **Critérios a confirmar com o usuário:** só primeiras divisões (o Opta mede clubes, e a Championship inglesa pode aparecer bem colocada) e uma liga por país
  - **Custo:** ~300–700 partidas por liga nova e temporada, com lotes de 20 → poucas centenas de requisições por temporada
- **5I:** ranking de cada competição em cada temporada, calculado só com as partidas daquela competição (usa `PlayerRankingSpec.CompetitionId`, já previsto). Sem custo de API: deriva dos dados já importados. Regras a definir **com o usuário**: corte de elegibilidade proporcional (ex.: % dos jogos disputados pelo time na competição, pois uma liga tem 34–38 rodadas e uma Copa do Mundo no máximo 7) e fator de presença proporcional à duração da competição. Dentro de uma competição o peso do torneio fica praticamente constante (exceto Champions: mata-mata × fase de liga). Frontend: seletor de competição ao lado do de temporada no ranking. Fazer depois da 5F.

### Decisões do usuário para as próximas etapas (25/09/2026)
- **5C:** clubes de fora da elite (adversários nas copas, clubes pequenos da Copa de Clubes) entram com Elo fixo de **1.400**, documentado em `/formula`
- **5D:** só **primeiras divisões**, **uma liga por país**. Libertadores e Sul-Americana com peso **bem menor que a Champions** (valor a propor na 5D). Ainda em aberto: fórmula do peso das ligas (proposta: proporcional ao rating do Opta, Premier League = 1,10) e renovação do plano Pro (vence em 25/10/2026)
- **5E:** **subir o corte de elegibilidade** com as temporadas completas (proposta: 20 partidas e 1.500 minutos; confirmar os números na 5E)
- **5I:** corte por competição de **50% dos jogos do time** na competição

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

