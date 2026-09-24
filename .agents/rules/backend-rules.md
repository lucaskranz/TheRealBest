# Rules: Backend (.NET / C#)

## Arquitetura
- SEMPRE seguir Clean Architecture com separação rigorosa: Domain → Application → Infrastructure → API
- O Domain NUNCA deve referenciar pacotes externos (exceto tipos primitivos do .NET)
- Application depende APENAS de Domain
- Scoring depende APENAS de Domain (é um módulo isolado)
- Infrastructure implementa as interfaces definidas em Domain
- API é a composição raiz (Dependency Injection)

## Entidades e Domínio
- Usar GUID como tipo de ID para todas as entidades
- Entidades devem ter construtores privados ou protegidos para EF Core
- Value Objects devem ser imutáveis (records ou classes com init-only properties)
- Enums devem representar conceitos do domínio (PlayerPosition, CompetitionTier, ActionType)
- Nullable reference types habilitados em todos os projetos

## Banco de Dados (EF Core)
- Code First com Fluent API (nunca Data Annotations nas entidades)
- Configurações em classes separadas (IEntityTypeConfiguration<T>)
- Todas as entidades DEVEM ter campos created_at e updated_at
- Campos JSONB para dados semi-estruturados (action_breakdown, penalty_breakdown)
- Índices compostos em tabelas de alta consulta (match_player_stats, season_rankings)

## API REST
- Controllers devem ser finos: delegar toda lógica para Use Cases
- Usar DTOs para entrada e saída (nunca expor entidades de domínio)
- Versionamento via URL (/api/v1/...)
- Paginação obrigatória em endpoints de listagem
- Respostas padronizadas com envelope: { data, meta, errors }
- Tratamento global de exceções via ExceptionHandlingMiddleware

## Localização (i18n)
- Labels estáticos: arquivos .resx por locale (Messages, ActionLabels, Positions)
- Conteúdo dinâmico: tabelas de tradução no banco (competition_translations, action_type_translations)
- Middleware de localização lê Accept-Language e seta CultureInfo
- NUNCA hardcodar strings visíveis ao usuário em controllers ou use cases
- Recursos em src/TheRealBest.API/Resources: o .resx neutro contém o pt-BR (padrão e fallback); .en.resx e .es.resx são satélites
- ActionLabels usa as chaves de ActionCatalog (snake_case) com a forma plural em "{chave}_plural"; toda ação nova precisa de rótulo nos 3 idiomas (LocalizationTests garante)
- Textos via ITranslationService (implementado por ResxTranslationService); o locale da requisição vem de SupportedLocales.Current
- LocalizationMiddleware roda ANTES do ExceptionHandlingMiddleware (a cultura definida por um middleware interno não volta para o externo)
- DTOs expõem o código (ex.: "CDM") e o rótulo localizado (ex.: PrimaryPositionLabel) lado a lado; nomes de competição via Competition.NameFor(locale)

## Background Services
- Implementar IHostedService para workers de longa duração
- Usar IServiceScopeFactory para criar scopes dentro dos workers
- Logging estruturado com Serilog em todos os workers
- Retry policies com Polly para chamadas a APIs externas (só erros transitórios 5xx; nunca repetir HTTP 429)
- APIs externas com limite por minuto (API-Football): requisições sequenciais via ApiFootballRequestPacer, nunca em paralelo — exceder o limite pode bloquear a conta

## Testes
- Testes unitários para toda lógica do Scoring Engine
- Testes de integração para repositories com banco in-memory ou Testcontainers
- Naming convention: MetodoTestado_Cenario_ResultadoEsperado
- Usar FluentAssertions para assertions legíveis
