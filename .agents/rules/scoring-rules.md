# Rules: Motor de Pontuação (Scoring Engine)

## Princípios Fundamentais
- O módulo TheRealBest.Scoring é COMPLETAMENTE ISOLADO — depende apenas de TheRealBest.Domain
- Toda lógica de cálculo de pontuação DEVE estar neste módulo (nunca em controllers ou repositories)
- Os pesos são carregáveis do banco de dados via ScoringRulesProvider (permite ajustes sem deploy)
- O algoritmo deve ser determinístico: mesmos inputs SEMPRE produzem o mesmo output

## Estrutura Obrigatória
- WeightMatrices/: uma classe por posição (GoalkeeperWeights, CenterBackWeights, FullBackWeights, DefensiveMidWeights, AttackingMidWeights, WingerWeights, StrikerWeights)
- Multipliers/: TournamentMultiplier, OpponentStrengthMultiplier, ClutchFactorMultiplier
- Calculators/: MatchPerformanceCalculator (MPS), SeasonScoreCalculator (FSS), MinutesFactorCalculator

## Regras de Cálculo
- Base de toda partida: 50.0 pontos
- MPS varia de 0 a 100 (clamped)
- Gols de atacantes valem MENOS que gols de defensores (pesos invertidos por responsabilidade posicional)
- Clean Sheet tem peso alto para defensores e goleiros, zero para atacantes
- Grandes chances perdidas têm penalidade severa para atacantes (-8 pts), leve para defensores (-2 pts)
- Fator Clutch: ações em jogos equilibrados (empate ou 1 gol) valem 25% mais; ações em goleadas valem 30% menos
- FatorMinutos: jogadores com < 60 min recebem proporcionalmente; jogadores com < 20 min só pontuam se tiverem ação decisiva no placar

## Multiplicadores de Contexto
- MultContexto = W_torneio × W_adversário × W_clutch
- W_torneio: Copa do Mundo Final (1.40) > UCL Knockout (1.35) > Top Leagues (1.10) > Copas Nacionais (1.05)
- W_adversário: Top 10 ELO (1.20) > Top 30 (1.10) > Meio tabela (1.00) > Rebaixamento (0.90)
- W_clutch: Placar equilibrado últimos 15min (1.25) > Normal (1.00) > Junk time >=3 gols (0.70)

## Ranking Sazonal (FSS)
- FSS = (Σ MPS_i × W_torneio_i) × FatorPresença
- FatorPresença = min(1.0, MinutosJogados / 2200)^0.5
- 2200 minutos ≈ 25 jogos completos (protege contra amostras pequenas)

## Auditabilidade Total
- O MatchPerformanceScore DEVE armazenar action_breakdown e penalty_breakdown como JSONB
- Cada linha do breakdown contém: action_key, label, count, weight, points, minute (quando aplicável)
- O frontend renderiza EXATAMENTE o que está salvo — NUNCA recalcula no cliente
- O campo algorithm_version permite rastrear qual versão da fórmula gerou o score

## Testes Obrigatórios
- Cenários com jogadores reais de diferentes posições
- Teste que um zagueiro com clean sheet + 5 duelos aéreos > atacante com 1 gol em junk time
- Teste que gol de atacante em empate aos 88min pontua mais que hat-trick em goleada 6x0
- Teste de edge cases: jogador com 0 minutos, prorrogação, cartão vermelho no 1º minuto
- Testes de snapshot: garantir que mudanças no algoritmo não alterem scores históricos involuntariamente
