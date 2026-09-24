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

$$\text{FSS} = \Bigg(\sum_{i=1}^{N} \text{MPS}_i \times \mathbf{W}_{\text{torneio}, i}\Bigg) \times \text{FatorPresença}$$

Onde:
* $N$ = Total de partidas disputadas na temporada.
* **Fator de Presença / Durabilidade:**
  $$\text{FatorPresença} = \min\left(1.0, \frac{\text{Minutos Jogados}}{2200}\right)^{0.5}$$
  *(Garante que atletas que jogaram pelo menos 2200 minutos (~25 jogos completos) atinjam o multiplicador máximo de 1.0, premiando a consistência e protegendo contra amostras pequenas sem superpunir por lesões curtas).*

---

## 7. Próxima Etapa: Validação Prática

Com essa modelagem fechada, podemos simular cenários reais:
1. Comparar **Rodri vs Vinicius Jr vs Bellingham** na temporada 2023/24 e ver como a fórmula se comporta.
2. Comparar um zagueiro de elite (ex: **Van Dijk / Rüdiger**) com um centroavante de ponta (ex: **Haaland / Kane**).
