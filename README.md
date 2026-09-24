# The Real Best — Beyond The Ballon

> **Futebol sem grife. Apenas mérito matemático.**  
> O primeiro ranking global 100% auditável de jogadores de futebol, baseado em performance jogo a jogo.

---

## 🌟 Sobre o Projeto

As premiações tradicionais de futebol (como a Bola de Ouro e o The Best da FIFA) sofrem historicamente de:
1. **Viés de popularidade e grife:** Jogadores da mídia ou de grande engajamento social recebem votos sem correspondência de campo.
2. **Viés pró-atacante:** Dificuldade em quantificar posicionamento defensivo, controle de ritmo e cortes providenciais em comparação a gols.
3. **Distorção de  Junk Time:** Fazer o 5º gol numa vitória de 6 a 0 contra um time rebaixado costuma ter peso idêntico ao gol da vitória aos 88 minutos na final de uma Champions League.

O **The Real Best** elimina essas distorções com um motor algorítmico baseado em dados avançados de partidas, onde qualquer torcedor pode auditar o extrato itemizado de cada ponto conquistado ou perdido.

---

## 📐 Estrutura do Algoritmo

Para cada partida disputada, o atleta obtém o **MPS (Match Performance Score)** de 0 a 100:

\text{MPS} = \text{Clamp}\Big(0, 100, \big(\text{Base} + \Delta\text{Ações} \times \text{FatorMinutos}\big) \times \text{MultContexto}\Big)

### Pesos por Posição:
- **Atacantes (ST/CF):** Julgados por letalidade (conversão de xG), retenção de pivô e finalizações; fortemente penalizados por grandes chances claras perdidas (-8 pts).
- **Pontas (LW/RW):** Avaliados pelo desequilíbrio no 1v1, dribles no terço final e ações criadoras de finalização (SCA).
- **Meias Ofensivos (CAM):** Avaliados por assistências esperadas (xA), quebras de linha e passes em profundidade.
- **Volantes Centrais (CDM/CM):** Pontuam por passes progressivos sob pressão, retenção de ritmo e desarmes que interrompem contra-ataques.
- **Defensores (CB/FB):** Recompensados por Clean Sheets (+16 pts), desarmes como último homem e bloqueios de chutes a gol; penalizados com rigor por erros que geram gol adversário (-25 pts).
- **Goleiros (GK):** Avaliados por defesas difíceis, saídas aéreas, pênaltis defendidos e gols prevenidos (xGOT - Gols sofridos).

A especificação completa está documentada em [fair_ranking_formula_specification.md](./fair_ranking_formula_specification.md).

---

## 🚀 Como Executar Localmente

Como o projeto é construído em Vanilla Web moderno, você não precisa de builds complexos:

`ash
# 1. Navegue até a pasta
cd C:\Repositorios\Pessoal\TheRealBest

# 2. Inicie um servidor HTTP simples (exemplo em Python):
python -m http.server 3000

# 3. Abra no navegador:
# http://localhost:3000
`

---

## 🛠️ Tecnologias Utilizadas
- **HTML5 Semântico:** Estrutura e acessibilidade.
- **Vanilla CSS (Design System próprio):** Dark mode premium, glassmorphism, tipografia *Cabinet Grotesk* + *Inter*.
- **JavaScript ES6+:** Motor reativo de dados, auditoria de extratos, filtros posicionais e comparativos.
