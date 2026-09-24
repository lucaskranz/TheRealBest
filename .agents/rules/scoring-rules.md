# Rules: Motor de Pontuação (Scoring Engine)

> A fórmula em vigor é a da seção 7 de docs/fair_ranking_formula_specification.md (algoritmo v1). Ela prevalece sobre as seções anteriores daquele documento.

## Princípios Fundamentais
- O módulo TheRealBest.Scoring é COMPLETAMENTE ISOLADO — depende apenas de TheRealBest.Domain
- Toda lógica de cálculo de pontuação DEVE estar neste módulo (nunca em controllers ou repositories)
- Os pesos padrão ficam em código (WeightMatrices/) e são servidos por IScoringRulesProvider; um provider que leia a tabela scoring_weights pode substituí-lo sem alterar o motor (ScoringRuleSet.FromWeights)
- Toda alteração de peso, linha de base ou fórmula exige incrementar a versão do algoritmo (ScoringRulesProvider.DefaultVersion)
- O algoritmo deve ser determinístico: mesmos inputs SEMPRE produzem o mesmo output (horário via TimeProvider; arredondamento sempre MidpointRounding.AwayFromZero)

## Estrutura Obrigatória
- WeightMatrices/: uma classe por posição (GoalkeeperWeights, CenterBackWeights, FullBackWeights, DefensiveMidWeights, AttackingMidWeights, WingerWeights, StrikerWeights); CDM e CM usam DefensiveMidWeights
- Multipliers/: TournamentMultiplier, OpponentStrengthMultiplier, ClutchFactorMultiplier
- Calculators/: MatchPerformanceCalculator (MPS), SeasonScoreCalculator (FSS), MinutesFactorCalculator, StatsActionMapper (estatística → ação)
- Rules/: ActionCatalog (metadados das ações), ScoringRuleSet (pesos + linhas de base versionados), ScoringRulesProvider

## Regras de Cálculo
- MPS = Clamp(0, 100, 50 + (ΔAções − LinhaDeBase_posição × FatorMinutos) × MultContexto)
- O contexto multiplica só o desempenho: uma atuação neutra vale 50 em qualquer jogo
- A linha de base da posição faz uma atuação média valer 50 em qualquer posição (valores provisórios até a calibração da 3C)
- Gols de atacantes valem MENOS que gols de defensores (pesos invertidos por responsabilidade posicional)
- Clean Sheet tem peso alto para defensores e goleiros, zero para atacantes
- Grandes chances perdidas têm penalidade severa para atacantes (-8 pts), leve para defensores (-2 pts)
- FatorMinutos: < 60 min = M/90; 60–90 = 1.0; prorrogação = 1 + (M−90)/180. Incide só sobre ações de volume; eventos (gols, cartões, erros...) valem integralmente
- Jogadores com < 20 min só entram no FSS se tiverem ação decisiva no placar (CountsTowardsSeason)

## Multiplicadores de Contexto
- MultContexto = W_torneio × W_adversário × W_clutch
- W_torneio: Copa do Mundo (1.40) > UCL mata-mata (1.35) > Euro/Copa América (1.30) > UCL fase de liga (1.20) > Top 5 ligas (1.10) > Copas nacionais semi/final (1.05) > Outros (0.95)
- W_adversário (rating ClubElo): ≥ 1880 (1.20) > ≥ 1780 (1.10) > ≥ 1600 (1.00) > abaixo (0.90)
- W_clutch pelo placar final (enquanto não há eventos por minuto): diferença ≤ 1 (1.25) > 2 gols (1.00) > ≥ 3 gols, junk time (0.70)

## Ranking Sazonal (FSS)
- FSS = (Σ MPS_i × W_torneio_i / Σ W_torneio_i) × FatorPresença — média ponderada, escala 0–100
- FatorPresença = min(1.0, MinutosJogados / 2200)^0.5
- 2200 minutos ≈ 25 jogos completos (protege contra amostras pequenas)
- Elegibilidade: só entra no ranking quem tem no mínimo 10 partidas contadas E 900 minutos (SeasonScore.IsRankingEligible / season_rankings.is_ranking_eligible)
- Inelegíveis ficam salvos em season_rankings (perfil do jogador), mas não aparecem na listagem nem recebem OverallRank/PositionRank; as consultas de ranking DEVEM filtrar is_ranking_eligible

## Auditabilidade Total
- O MatchPerformanceScore DEVE armazenar action_breakdown e penalty_breakdown como JSONB
- Cada linha do breakdown contém: action_key, label, count, weight, points, minute (quando aplicável), minutes_factor
- O recibo guarda SubtotalRaw (soma dos itens), PositionBaseline e os três multiplicadores, de modo que o MPS seja recalculável a partir dele
- O frontend renderiza EXATAMENTE o que está salvo — NUNCA recalcula no cliente
- O campo algorithm_version permite rastrear qual versão da fórmula gerou o score

## Testes Obrigatórios
- Cenários com jogadores reais de diferentes posições (estatísticas aproximadas devem ser sinalizadas como tal)
- Teste que um zagueiro com clean sheet + 5 duelos aéreos > atacante com 1 gol em junk time (ambos partindo da linha típica da posição)
- Teste que o gol da vitória contra rival de topo em jogo equilibrado vale mais que o mesmo gol numa goleada de 6x0 contra time fraco
- Teste de edge cases: jogador com 0 minutos, prorrogação, cartão vermelho no 1º minuto
- Testes de snapshot: garantir que mudanças no algoritmo não alterem scores históricos involuntariamente
