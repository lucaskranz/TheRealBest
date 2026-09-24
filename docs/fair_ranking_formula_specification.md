# Especificação Matemática do Algoritmo: Fair Player Index (FPI)

## 1. Filosofia e Premissas do Algoritmo

O objetivo do **Fair Player Index (FPI)** é eliminar:
1. **O viés de notoriedade/grife:** Jogadores que têm grande apelo midiático mas entregam partidas protocolares não devem pontuar mais do que quem foi decisivo em campo.
2. **O viés pró-atacante:** Tradicionalmente, atacantes ganham prêmios individuais porque gols são mais fáceis de quantificar que posicionamento defensivo, controle de ritmo ou cortes providenciais.
3. **A distorção de "Junk Time" (minutos inúteis):** Fazer o 5º gol numa goleada de 6 a 0 contra um adversário fraco não pode ter o mesmo valor de marcar o gol da vitória aos 88 minutos contra um rival de topo.
4. **O dilema "Média vs. Acumulado":**
   - Se usarmos apenas a *média*, um jogador que fez 8 jogos perfeitos e se machucou o resto do ano venceria.
   - Se usarmos apenas a *soma bruta*, um jogador mediano que atuou em 55 partidas superaria um craque que jogou 35 partidas magistrais.
   - **Solução:** O FPI usa uma **Pontuação por Partida (MPS - Match Performance Score)** com um **Fator de Consistência e Volume Sazonal**.

---

## 2. Estrutura Matemática da Partida (Match Performance Score - MPS)

A cada partida disputada, o jogador obtém uma nota de performance que varia de **0 a 100** (onde 50 representa uma partida regular/neutra):

> **Implementação:** a fórmula em vigor (algoritmo v1) está na seção 7.1. Ela aplica o contexto só sobre o desempenho e desconta uma linha de base por posição.

$$\text{MPS} = \text{Clamp}\Big(0, 100, \big(\text{Base} + \Delta\text{Ações} \times \text{FatorMinutos}\big) \times \text{MultContexto}\Big)$$

Onde:
* **$\text{Base} = 50.0$** (nota inicial de quem entra em campo).
* **$\Delta\text{Ações}$**: Soma dos pontos positivos e negativos de eventos específicos ajustados para a posição do atleta.
* **$\text{FatorMinutos}$**: Evita que atletas com poucos minutos tenham distorções.
* **$\text{MultContexto}$**: Pondera a dificuldade do jogo, nível do torneio e importância do momento do lance.

---

## 3. Matriz de Ações por Posição ($\Delta\text{Ações}$)

Para garantir justiça, dividimos o futebol em **5 funções táticas principais**:
1. **GK (Goleiro)**
2. **CB (Zagueiro Central)**
3. **FB/WB (Lateral / Ala)**
4. **CDM/CM (Volante / Meio-Campista Central)**
5. **CAM/W (Meia-Atacante / Ponta)**
6. **ST/CF (Centroavante / Atacante de Área)**

### Tabela de Pesos por Ação (Pontos Brutos)

| Métrica / Ação | Goleiro (GK) | Zagueiro (CB) | Lateral (FB) | Volante/Meia (CM) | Meia-Atac/Ponta (CAM/W) | Centroavante (ST) |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Gol (Bola Rolando)** | +50 | +30 | +25 | +20 | +16 | +12 |
| **Gol (Pênalti)** | +30 | +15 | +14 | +12 | +10 | +8 |
| **Assistência Direta** | +25 | +16 | +14 | +12 | +10 | +8 |
| **Assistência Esperada (xA)** *(por unidade)* | +15 | +12 | +10 | +10 | +8 | +6 |
| **Grande Chance Criada** | +10 | +8 | +7 | +6 | +5 | +4 |
| **Finalização no Alvo** | — | +2 | +2 | +2.5 | +2.5 | +3 |
| **xG Superado (Gols - xG)** *(bônus de pontaria)* | — | +4 | +4 | +5 | +6 | +7 |
| **Passe Decisivo (Key Pass)** | +4 | +2 | +2.5 | +3 | +2.5 | +2 |
| **Passe Progressivo** *(terço final)* | +1.5 | +1.2 | +1.0 | +0.8 | +0.6 | +0.4 |
| **Precisão de Passe (%)** *(acima de 85%)* | +4 | +4 | +3 | +5 | +3 | +2 |
| **Drible Bem Sucedido** | — | +2 | +2.5 | +2.5 | +3 | +2.5 |
| **Falta Sofrida no Ataque** | — | +1 | +1.5 | +1.5 | +2 | +2 |
| **Desarme Ganho (Tackle)** | — | +4 | +3.5 | +3 | +2 | +1.5 |
| **Interceptação / Corte** | — | +3.5 | +3 | +2.5 | +1.5 | +1 |
| **Duelo Aéreo Ganho** | +1 | +2.5 | +1.8 | +1.8 | +1.2 | +1.8 |
| **Duelo no Chão Ganho** | — | +2 | +2 | +2 | +1.5 | +1.5 |
| **Recuperação de Posse** | +1 | +2 | +2 | +2.2 | +1.5 | +1 |
| **Clean Sheet (>60 min jogados)** | +18 | +14 | +10 | +4 | +0 | +0 |
| **Defesa Difícil (Dentro da Área)** | +6 | — | — | — | — | — |
| **Gols Prevenidos (xGOT - Gols Sofridos)** | +12 | — | — | — | — | — |
| **Pênalti Defendido** | +25 | — | — | — | — | — |
| **Saída Aérea com Sucesso (Claim)** | +4 | — | — | — | — | — |

---

### Tabela de Penalidades e Deduções (Pontos Negativos)

| Falha / Evento Negativo | GK | CB | FB | CM | CAM/W | ST |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Erro Grave que levou a Gol** | -25 | -22 | -20 | -18 | -12 | -10 |
| **Erro que levou a Finalização Rival** | -10 | -8 | -8 | -6 | -4 | -3 |
| **Pênalti Cometido** | -18 | -15 | -15 | -12 | -10 | -8 |
| **Gol Contra** | -18 | -16 | -15 | -12 | -10 | -10 |
| **Cartão Amarelo** | -3 | -3 | -3 | -3 | -3 | -3 |
| **Cartão Vermelho Direto** | -25 | -25 | -25 | -25 | -25 | -25 |
| **Grande Chance Perdida (Big Chance Missed)** | — | -2 | -3 | -4 | -6 | -8 |
| **Perda da Posse no Campo Defensivo** | -8 | -6 | -5 | -4 | -2 | -1 |
| **Impedimento Evitável** | — | — | -1 | -1 | -1.5 | -2 |
| **Gols Sofridos pelo Time (após o 1º)** | -4 | -3 | -2.5 | -1 | 0 | 0 |

---

## 4. Multiplicadores de Contexto ($\text{MultContexto}$)

Uma grande atuação em um clássico de Champions League vale mais que contra o lanterna da liga local.

$$\text{MultContexto} = \mathbf{W}_{\text{torneio}} \times \mathbf{W}_{\text{adversário}} \times \mathbf{W}_{\text{clutch}}$$

### 4.1. Peso do Torneio ($\mathbf{W}_{\text{torneio}}$)
* **Copa do Mundo (Fases Finais / Mata-mata):** 1.40
* **UEFA Champions League (Mata-mata):** 1.35
* **Eurocopa / Copa América (Fases Finais):** 1.30
* **Champions League (Fase de Grupos / Fase Liga):** 1.20
* **Top 5 Ligas Nacionais (Premier League, La Liga, Serie A, Bundesliga, Ligue 1):** 1.10
* **Copas Nacionais (FA Cup, Copa del Rey, DFB Pokal - Finais/Semis):** 1.05
* **Fases Iniciais de Copas / Outros torneios:** 0.95

### 4.2. Peso da Força do Adversário ($\mathbf{W}_{\text{adversário}}$)
Baseado no **Clube ELO Ranking** ou classificação do rival:
* **Contra Top 10 Mundial (Ex: Real Madrid, Man City, Bayern):** 1.20
* **Contra Top 11-30 Mundial:** 1.10
* **Adversário de Meio de Tabela:** 1.00
* **Adversário da Zona de Rebaixamento / Divisões inferiores:** 0.90

### 4.3. O Fator Decisivo / "Clutch" ($\mathbf{W}_{\text{clutch}}$)
Calculado lance a lance para ações capitais (Gols, Assistências, Desarmes como último homem, Defesas cara a cara):
* **Com placar empatado ou diferença de 1 gol nos últimos 15 min:** $\times 1.25$
* **Com o placar equilibrado (jogo aberto):** $\times 1.00$
* **Com o jogo decidido (vantagem $\ge 3$ gols):** $\times 0.70$ *(reduz o valor de inflar números em goleadas)*

---

## 5. Normalização por Minutos Jogados ($\text{FatorMinutos}$)

Um jogador que entra aos 85 minutos e faz um desarme não pode receber a pontuação de quem manteve o ritmo por 90 minutos:

$$\text{FatorMinutos} = \begin{cases} 
\frac{M}{90}, & \text{se } M < 60 \\
1.0, & \text{se } 60 \le M \le 90 \\
1.0 + \frac{M - 90}{180}, & \text{se } M > 90 \text{ (Prorrogação)}
\end{cases}$$

*(Onde $M$ são os minutos oficiais em campo).* Jogadores com menos de 20 minutos jogados só entram no ranking se tiverem tido uma ação decisiva com impacto direto no placar.

---

## 6. O Ranking da Temporada: Fair Season Score (FSS)

Como consolidamos as 40 a 60 partidas de um ano em um ranking final incontestável?

> **Implementação:** o FSS em vigor (algoritmo v1) é a média ponderada da seção 7.3.

$$\text{FSS} = \Bigg(\sum_{i=1}^{N} \text{MPS}_i \times \mathbf{W}_{\text{torneio}, i}\Bigg) \times \text{FatorPresença}$$

Onde:
* $N$ = Total de partidas disputadas na temporada.
* **Fator de Presença / Durabilidade:**
  $$\text{FatorPresença} = \min\left(1.0, \frac{\text{Minutos Jogados}}{2200}\right)^{0.5}$$
  *(Garante que atletas que jogaram pelo menos 2200 minutos (~25 jogos completos) atinjam o multiplicador máximo de 1.0, premiando a consistência e protegendo contra amostras pequenas sem superpunir por lesões curtas).*

---

## 7. Decisões de Implementação (Algoritmo v1)

Ao implementar e testar as fórmulas acima (Etapas 2A/2B), cinco pontos se mostraram inconsistentes com a própria filosofia da seção 1. **A versão implementada é a abaixo, e ela prevalece sobre as seções 2, 5 e 6 quando houver conflito.**

### 7.1. Fórmula do MPS implementada

$$\text{MPS} = \text{Clamp}\Big(0, 100, \text{Base} + \big(\Delta\text{Ações} - \text{LinhaDeBase}_{\text{posição}} \times \text{FatorMinutos}\big) \times \text{MultContexto}\Big)$$

| # | Problema na fórmula original | Decisão v1 |
| :--- | :--- | :--- |
| 1 | `(Base + Δ) × MultContexto` multiplica também a base 50. Num mata-mata de Champions contra time top com placar apertado (×2.0), qualquer atuação neutra vira 100; numa goleada contra time fraco, cai para ~35. | O contexto multiplica **só o desempenho**. Uma atuação neutra vale 50 em qualquer jogo, e o contexto amplifica acertos e erros. |
| 2 | Os pesos de ações de volume (recuperações, duelos, passes) somam Δ ≈ +50 para um volante **médio**, e o MPS satura em 100 para quase todo titular. Posições com mais volume (volantes, zagueiros) dominariam o ranking. | **Linha de base por posição**: desconta-se o Δ esperado de uma atuação média na posição, proporcional aos minutos. A atuação média vale 50 em qualquer posição. |
| 3 | O FatorMinutos multiplicava todo o Δ: o gol decisivo de quem entrou aos 80' valeria 1/9 de um gol, e um vermelho no 1º minuto quase não pesaria. | O FatorMinutos incide **só sobre ações de volume**. Eventos pontuais (gols, assistências, cartões, erros, pênaltis, clean sheet, gols sofridos) valem integralmente. |
| 4 | O W_clutch é definido lance a lance (placar e minuto de cada ação), mas a ingestão ainda não traz eventos por minuto. | Enquanto não houver eventos, o W_clutch vem do **placar final**: diferença ≤ 1 gol → 1.25; diferença de 2 → 1.00; diferença ≥ 3 → 0.70. |
| 5 | O FSS como soma `Σ MPS × W` premia volume: 60 jogos com nota 50 superam 35 jogos com nota 70, contrariando a seção 1. | O FSS é a **média ponderada** (seção 7.3). |

### 7.2. Linhas de base provisórias (v1)

Δ esperado de uma atuação média de 90 minutos, estimado aplicando médias típicas de titulares das 5 grandes ligas (incluindo a probabilidade de gol, assistência e clean sheet) às matrizes de peso da seção 3:

| GK | CB | FB | CDM/CM | CAM/W | ST |
| :---: | :---: | :---: | :---: | :---: | :---: |
| 22 | 41 | 43 | 54 | 44 | 27 |

Estes valores são **provisórios** e devem ser recalibrados com dados reais na Etapa 3C, o que gera o algoritmo v2.

### 7.3. FSS implementado

$$\text{FSS} = \frac{\sum_{i=1}^{N} \text{MPS}_i \times \mathbf{W}_{\text{torneio}, i}}{\sum_{i=1}^{N} \mathbf{W}_{\text{torneio}, i}} \times \text{FatorPresença}$$

Escala de 0 a 100. Partidas com menos de 20 minutos e sem ação decisiva no placar (gol, assistência, pênalti defendido ou cometido, erro que levou a gol, gol contra, cartão vermelho) não entram no FSS.

**Elegibilidade para o ranking:** o jogador só aparece no ranking com **no mínimo 10 partidas contadas e 900 minutos** na temporada. Quem não atinge o corte tem o FSS calculado e salvo (para o perfil), mas não recebe posição no ranking. O FatorPresença continua valendo para os elegíveis, entre 900 e 2200 minutos.

### 7.4. Outras definições

* **Posições → matrizes:** CDM e CM usam a coluna "Volante/Meia"; CAM e W usam a coluna "Meia-Atac/Ponta".
* **W_torneio:** Copa do Mundo 1.40 em toda a fase final; Eurocopa/Copa América 1.30; Champions 1.35 no mata-mata e 1.20 na fase de liga; Top 5 ligas 1.10; copas nacionais 1.05 em semifinal/final e 0.95 antes disso; demais 0.95.
* **W_adversário (rating ClubElo):** ≥ 1880 → 1.20 (top 10); ≥ 1780 → 1.10 (top 30); ≥ 1600 → 1.00; abaixo → 0.90. Times sem Elo ingerido ficam com o padrão 1500 (0.90).
* **Precisão de passe:** bônus único se > 85% com pelo menos 20 passes. **Clean sheet:** só com mais de 60 minutos. **xG superado:** bônus apenas quando Gols − xG > 0.
* **Ações sem fonte de dados hoje** (ficam de fora até a ingestão fornecê-las): gols prevenidos (xGOT), saídas aéreas, erro que levou a finalização, gol contra e perda de posse no campo defensivo (a fonte só informa o total de perdas). **Defesas** usam o total de defesas, não só as difíceis dentro da área.
* **Arredondamento:** 4 casas nos itens e multiplicadores e 2 casas no MPS, sempre `MidpointRounding.AwayFromZero`.

---

## 8. Validação Prática

Os testes em `tests/TheRealBest.Scoring.Tests` simulam partidas reais (Rodri na final da UCL 2023, Vinícius Jr na final de 2024, os 5 gols de Haaland contra o Leipzig em 2023 e Rüdiger contra o City nas quartas de 2024), com estatísticas aproximadas. Próximos passos, com dados reais (Etapa 3C):
1. Comparar **Rodri vs Vinicius Jr vs Bellingham** na temporada 2023/24.
2. Comparar um zagueiro de elite (ex: **Van Dijk / Rüdiger**) com um centroavante de ponta (ex: **Haaland / Kane**).
3. Recalibrar as linhas de base da seção 7.2.
