# Rules: Banco de Dados e Dados

## PostgreSQL
- Usar UUID (gen_random_uuid()) como chave primária em todas as tabelas
- Convenção de nomes: snake_case para tabelas e colunas
- Todas as tabelas DEVEM ter created_at (timestamp with time zone, default now())
- Tabelas que sofrem atualização DEVEM ter updated_at
- Campos JSONB para dados semi-estruturados (extratos de pontuação)
- Índices compostos nas tabelas mais consultadas:
  - match_player_stats: (match_id, player_id)
  - match_performance_scores: (player_id, match_id), (player_id, algorithm_version)
  - season_rankings: (season_year, overall_rank), (season_year, position_rank)
- Tabelas de tradução usam constraint UNIQUE(entity_id, locale)

## EF Core
- Fluent API exclusivamente (NUNCA Data Annotations nas entidades de domínio)
- Cada entidade tem sua configuração em Infrastructure/Data/Configurations/
- Usar HasConversion para Enums armazenados como string
- Propriedades JSONB mapeadas com ToJson() do EF Core 8
- Migrations nomeadas descritivamente: YYYYMMDD_NomeDaMigracao

## Dados Externos
- Dados da API-Football são mapeados para entidades de domínio via ApiFootballMapper
- O external_api_id em cada entidade permite correlação com a fonte externa
- Dados brutos NUNCA são descartados — match_player_stats guarda tudo que a API retorna
- Dados calculados (MPS, FSS) ficam em tabelas separadas com referência ao algorithm_version
