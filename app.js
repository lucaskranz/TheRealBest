/**
 * THE REAL BEST — BEYOND THE BALLON
 * Interactive Engine & Auditing Dashboard
 */

// Player Database (2023/24 Season Audit)
const PLAYERS_DATA = [
  {
    id: "rodri",
    rank: 1,
    name: "Rodri",
    team: "Manchester City / Espanha",
    posCategory: "CM/CDM",
    posLabel: "Volante Central (CDM)",
    league: "Premier League / Euro 2024",
    avatar: "https://images.unsplash.com/photo-1570295999919-56ceb5ecca61?w=150&auto=format&fit=crop&q=80",
    games: 50,
    minutes: 4325,
    mpsAvg: 86.4,
    clutchIndex: 94.2,
    fssScore: 4320,
    ballonRank: 1,
    realBestRank: 1,
    discrepancyNote: "Consenso: Campeão da Euro, Premier League e Mundial com apenas 1 derrota em 50 jogos e domínio absoluto do ritmo em todas as competições.",
    summary: "O termômetro do futebol moderno. Maior precisão de passe sob pressão da Europa, 9 gols cruciais em jogos empatados e liderança absoluta em recuperações no meio-campo.",
    matches: [
      {
        opponent: "Espanha 2 x 1 Inglaterra (Final Euro 2024)",
        date: "14 Jul 2024",
        competition: "Eurocopa (Final)",
        minutes: 45,
        finalScore: 88.5,
        receipt: {
          base: 50.0,
          actions: [
            { label: "Precisão de Passe (93% - 41 de 44 certos)", pts: "+6.0" },
            { label: "4 Bolas Recuperadas no Meio-Campo", pts: "+12.0" },
            { label: "2 Desarmes que travaram contra-ataque", pts: "+10.0" },
            { label: "1 Interceptação crucial na entrada da área", pts: "+5.0" }
          ],
          penalties: [
            { label: "Substituição por lesão no intervalo", pts: "0.0" }
          ],
          context: [
            { label: "Multiplicador de Torneio (Final Euro)", mult: "x 1.30" },
            { label: "Adversário de Topo Mundial (Inglaterra)", mult: "x 1.15" }
          ]
        }
      },
      {
        opponent: "Man City 3 x 1 West Ham (Decisão Premier League)",
        date: "19 Mai 2024",
        competition: "Premier League (Rodada Final)",
        minutes: 90,
        finalScore: 92.4,
        receipt: {
          base: 50.0,
          actions: [
            { label: "1 Gol decisivo de fora da área (selou o título)", pts: "+20.0" },
            { label: "112 Passes certos (94% de precisão sob pressão)", pts: "+7.0" },
            { label: "3 Passes progressivos no terço final", pts: "+6.0" },
            { label: "6 Bolas recuperadas", pts: "+18.0" }
          ],
          penalties: [
            { label: "1 Perda de posse forçada", pts: "-4.0" }
          ],
          context: [
            { label: "Fator Decisivo (Jogo do Título aos 59')", mult: "x 1.25" },
            { label: "Multiplicador Premier League", mult: "x 1.10" }
          ]
        }
      }
    ]
  },
  {
    id: "vinicius-jr",
    rank: 2,
    name: "Vinícius Júnior",
    team: "Real Madrid / Brasil",
    posCategory: "ST/W",
    posLabel: "Ponta Esquerda (LW)",
    league: "La Liga / Champions League",
    avatar: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80",
    games: 39,
    minutes: 3080,
    mpsAvg: 85.8,
    clutchIndex: 96.5,
    fssScore: 4180,
    ballonRank: 2,
    realBestRank: 2,
    discrepancyNote: "Altíssimo impacto em mata-matas da UCL (gols e assistências contra Bayern, Dortmund e City), porém com menor minutagem total que Rodri.",
    summary: "O jogador mais desequilibrante do planeta no mano a mano no terço final. Decisivo nas semifinais e final da Champions League.",
    matches: [
      {
        opponent: "Real Madrid 2 x 0 Borussia Dortmund (Final UCL)",
        date: "01 Jun 2024",
        competition: "Champions League (Final)",
        minutes: 90,
        finalScore: 96.8,
        receipt: {
          base: 50.0,
          actions: [
            { label: "1 Gol decisivo aos 83' (bola rolando)", pts: "+14.0" },
            { label: "5 Dribles 1v1 completados no terço final (5x +4)", pts: "+20.0" },
            { label: "3 Ações criadoras de finalização (3x +3)", pts: "+9.0" },
            { label: "2 Faltas sofridas no ataque", pts: "+4.0" }
          ],
          penalties: [
            { label: "1 Grande chance clara perdida", pts: "-6.0" },
            { label: "1 Cartão Amarelo por reclamação", pts: "-3.0" }
          ],
          context: [
            { label: "Final de Champions League", mult: "x 1.35" },
            { label: "Fator Decisivo (Gol sacramentou o título)", mult: "x 1.20" }
          ]
        }
      },
      {
        opponent: "Bayern 2 x 2 Real Madrid (Semi UCL Ida)",
        date: "30 Abr 2024",
        competition: "Champions League (Semifinal)",
        minutes: 90,
        finalScore: 94.0,
        receipt: {
          base: 50.0,
          actions: [
            { label: "2 Gols marcados (1 bola rolando, 1 pênalti)", pts: "+24.0" },
            { label: "3 Dribles bem sucedidos", pts: "+12.0" },
            { label: "Superação de xG na partida (+0.8xG)", pts: "+6.0" }
          ],
          penalties: [
            { label: "2 Perdas de posse no ataque", pts: "-4.0" }
          ],
          context: [
            { label: "Semi de Champions League fora de casa", mult: "x 1.35" },
            { label: "Rival Top 5 Europeu (Bayern)", mult: "x 1.15" }
          ]
        }
      }
    ]
  },
  {
    id: "bellingham",
    rank: 3,
    name: "Jude Bellingham",
    team: "Real Madrid / Inglaterra",
    posCategory: "CAM",
    posLabel: "Meia Ofensivo (CAM)",
    league: "La Liga / Champions League",
    avatar: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
    games: 42,
    minutes: 3640,
    mpsAvg: 83.2,
    clutchIndex: 95.0,
    fssScore: 4020,
    ballonRank: 3,
    realBestRank: 3,
    discrepancyNote: "Primeiro semestre avassalador com gols nos acréscimos (El Clásico), caindo ligeiramente de ritmo no 2º turno após atuar mais recuado.",
    summary: "Capacidade ímpar de pisar na área adversária e resolver jogos travados aos 90 minutos. Peça central na conquista de La Liga e vice na Euro.",
    matches: [
      {
        opponent: "Real Madrid 3 x 2 Barcelona (La Liga)",
        date: "21 Abr 2024",
        competition: "La Liga (El Clásico)",
        minutes: 90,
        finalScore: 93.5,
        receipt: {
          base: 50.0,
          actions: [
            { label: "1 Gol da vitória aos 91' (bola rolando)", pts: "+16.0" },
            { label: "1 Assistência Esperada (xA: 0.6)", pts: "+7.2" },
            { label: "3 Passes em profundidade certos", pts: "+15.0" },
            { label: "4 Duelos no chão ganhos", pts: "+8.0" }
          ],
          penalties: [
            { label: "1 Perda de posse por decisão errada", pts: "-3.0" }
          ],
          context: [
            { label: "Gol nos acréscimos contra o maior rival", mult: "x 1.25" },
            { label: "Multiplicador de Clássico Decisivo", mult: "x 1.15" }
          ]
        }
      }
    ]
  },
  {
    id: "kroos",
    rank: 4,
    name: "Toni Kroos",
    team: "Real Madrid / Alemanha",
    posCategory: "CM/CDM",
    posLabel: "Meia Armador Recuado (CM)",
    league: "La Liga / Euro 2024",
    avatar: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=150&auto=format&fit=crop&q=80",
    games: 48,
    minutes: 3450,
    mpsAvg: 84.1,
    clutchIndex: 89.0,
    fssScore: 3950,
    ballonRank: 9,
    realBestRank: 4,
    discrepancyNote: "GRANDE INJUSTIÇA DO BALLON D'OR: A votação tradicional o colocou em 9º por falta de gols, mas ele liderou o mundo em passes progressivos e controle de ritmo.",
    summary: "O maestro da temporada. 95% de precisão de passe ao longo de um ano inteiro e assistência magistral na final da Champions.",
    matches: [
      {
        opponent: "Real Madrid 2 x 0 Borussia Dortmund (Final UCL)",
        date: "01 Jun 2024",
        competition: "Champions League (Final)",
        minutes: 85,
        finalScore: 91.0,
        receipt: {
          base: 50.0,
          actions: [
            { label: "1 Assistência direta (escanteio medido para Carvajal)", pts: "+12.0" },
            { label: "97% de acerto em passes (76 de 78 certos)", pts: "+6.0" },
            { label: "9 Inversões longas de corredor perfeitas", pts: "+22.5" },
            { label: "2 Faltas táticas para travar contra-ataque", pts: "+4.0" }
          ],
          penalties: [],
          context: [
            { label: "Final de Champions League", mult: "x 1.35" }
          ]
        }
      }
    ]
  },
  {
    id: "van-dijk",
    rank: 5,
    name: "Virgil van Dijk",
    team: "Liverpool / Holanda",
    posCategory: "CB/FB",
    posLabel: "Zagueiro Central (CB)",
    league: "Premier League",
    avatar: "https://images.unsplash.com/photo-1522075469751-3a6694fb2f61?w=150&auto=format&fit=crop&q=80",
    games: 48,
    minutes: 4210,
    mpsAvg: 82.7,
    clutchIndex: 91.0,
    fssScore: 3890,
    ballonRank: "Fora do Top 20",
    realBestRank: 5,
    discrepancyNote: "ESCÂNDALO DO BALLON D'OR: Van Dijk sequer figurou entre os finalistas da premiação tradicional por jogar de zagueiro, apesar de liderar 82% de duelos aéreos ganhos.",
    summary: "A muralha da Premier League. Liderou a Europa em duelos aéreos vencidos (82%), 17 Clean Sheets e o gol do título da Carabao Cup aos 118'.",
    matches: [
      {
        opponent: "Chelsea 0 x 1 Liverpool (Final Carabao Cup)",
        date: "25 Fev 2024",
        competition: "Carabao Cup (Final)",
        minutes: 120,
        finalScore: 95.2,
        receipt: {
          base: 50.0,
          actions: [
            { label: "Clean Sheet em 120 minutos", pts: "+16.0" },
            { label: "1 Gol decisivo de cabeça aos 118'", pts: "+30.0" },
            { label: "7 Duelos aéreos defensivos ganhos (7x +3.5)", pts: "+24.5" },
            { label: "1 Desarme crucial como último homem", pts: "+10.0" }
          ],
          penalties: [
            { label: "1 Falta cometida na intermediária", pts: "-2.0" }
          ],
          context: [
            { label: "Final com gol do título na prorrogação", mult: "x 1.25" }
          ]
        }
      }
    ]
  },
  {
    id: "carvajal",
    rank: 6,
    name: "Dani Carvajal",
    team: "Real Madrid / Espanha",
    posCategory: "CB/FB",
    posLabel: "Lateral Direito (RB)",
    league: "La Liga / Euro 2024",
    avatar: "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=150&auto=format&fit=crop&q=80",
    games: 44,
    minutes: 3600,
    mpsAvg: 81.9,
    clutchIndex: 93.0,
    fssScore: 3840,
    ballonRank: 4,
    realBestRank: 6,
    discrepancyNote: "Consistência extraordinária: campeão de La Liga, UCL (com gol do título) e Eurocopa, provando que laterais têm impacto de primeiro nível.",
    summary: "O melhor lateral da temporada. Marcou o gol que abriu a vitória na final da Champions e neutralizou pontas de elite na Eurocopa.",
    matches: [
      {
        opponent: "Real Madrid 2 x 0 Borussia Dortmund (Final UCL)",
        date: "01 Jun 2024",
        competition: "Champions League (Final)",
        minutes: 90,
        finalScore: 94.8,
        receipt: {
          base: 50.0,
          actions: [
            { label: "1 Gol que abriu o placar aos 74'", pts: "+25.0" },
            { label: "Clean Sheet mantido", pts: "+10.0" },
            { label: "4 Duelos 1v1 ganhos contra Adeyemi", pts: "+16.0" },
            { label: "3 Desarmes na linha lateral", pts: "+10.5" }
          ],
          penalties: [],
          context: [
            { label: "Final de Champions League", mult: "x 1.35" }
          ]
        }
      }
    ]
  },
  {
    id: "haaland",
    rank: 7,
    name: "Erling Haaland",
    team: "Manchester City / Noruega",
    posCategory: "ST/W",
    posLabel: "Centroavante (ST)",
    league: "Premier League",
    avatar: "https://images.unsplash.com/photo-1492562080023-ab3db95bfbce?w=150&auto=format&fit=crop&q=80",
    games: 45,
    minutes: 3740,
    mpsAvg: 80.4,
    clutchIndex: 78.5,
    fssScore: 3760,
    ballonRank: 5,
    realBestRank: 7,
    discrepancyNote: "Artilheiro da Premier League com 27 gols, mas penalizado por 34 grandes chances perdidas e atuações apagadas contra o Real Madrid nas quartas da UCL.",
    summary: "Máquina de gols no campeonato inglês, mas nosso modelo o penalizou justamente pela alta taxa de chances perdidas e pouca participação fora da área.",
    matches: [
      {
        opponent: "Man City 5 x 1 Wolves",
        date: "04 Mai 2024",
        competition: "Premier League",
        minutes: 82,
        finalScore: 95.0,
        receipt: {
          base: 50.0,
          actions: [
            { label: "4 Gols marcados (2 bola rolando, 2 pênaltis)", pts: "+44.0" },
            { label: "6 Finalizações no alvo", pts: "+18.0" }
          ],
          penalties: [
            { label: "1 Grande chance perdida", pts: "-8.0" }
          ],
          context: [
            { label: "Jogo resolvido com folga (redução junk time)", mult: "x 0.85" }
          ]
        }
      },
      {
        opponent: "Man City 1 x 1 Real Madrid (Volta Quartas UCL)",
        date: "17 Abr 2024",
        competition: "Champions League (Quartas)",
        minutes: 90,
        finalScore: 54.0,
        receipt: {
          base: 50.0,
          actions: [
            { label: "1 Duelo aéreo ganho", pts: "+3.5" },
            { label: "1 Chute no travessão", pts: "+2.0" }
          ],
          penalties: [
            { label: "Partida fantasma (<20 toques na bola em 90')", pts: "-10.0" },
            { label: "2 Grandes chances desperdiçadas", pts: "-16.0" }
          ],
          context: [
            { label: "Quartas de Final UCL contra Real Madrid", mult: "x 1.25" }
          ]
        }
      }
    ]
  },
  {
    id: "wirtz",
    rank: 8,
    name: "Florian Wirtz",
    team: "Bayer Leverkusen / Alemanha",
    posCategory: "CAM",
    posLabel: "Meia Criador (CAM)",
    league: "Bundesliga / Euro 2024",
    avatar: "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=150&auto=format&fit=crop&q=80",
    games: 49,
    minutes: 3620,
    mpsAvg: 81.5,
    clutchIndex: 92.0,
    fssScore: 3740,
    ballonRank: 12,
    realBestRank: 8,
    discrepancyNote: "Protagonista do título invicto histórico do Leverkusen com 18 gols e 20 assistências, mas subestimado pela mídia internacional.",
    summary: "O cérebro da maior zebra invicta do futebol alemão. Mestre em assistências esperadas (xA) e quebras de linha entre os volantes adversários.",
    matches: [
      {
        opponent: "Leverkusen 5 x 0 Werder Bremen (Título Bundesliga)",
        date: "14 Abr 2024",
        competition: "Bundesliga",
        minutes: 45,
        finalScore: 94.0,
        receipt: {
          base: 50.0,
          actions: [
            { label: "Hat-trick histórico vindo do banco", pts: "+48.0" },
            { label: "3 Dribles completados", pts: "+9.0" }
          ],
          penalties: [],
          context: [
            { label: "Jogo do título histórico inédito", mult: "x 1.15" }
          ]
        }
      }
    ]
  },
  {
    id: "martinez",
    rank: 9,
    name: "Emiliano Martínez",
    team: "Aston Villa / Argentina",
    posCategory: "GK",
    posLabel: "Goleiro (GK)",
    league: "Premier League / Copa América",
    avatar: "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&auto=format&fit=crop&q=80",
    games: 47,
    minutes: 4180,
    mpsAvg: 80.8,
    clutchIndex: 94.5,
    fssScore: 3690,
    ballonRank: 18,
    realBestRank: 9,
    discrepancyNote: "Campeão da Copa América sofrendo apenas 1 gol em todo o torneio e herói da classificação do Aston Villa para a Champions League.",
    summary: "Líder absoluto em gols prevenidos (xGOT) em pênaltis e mata-matas. Elevou o patamar do Aston Villa na Premier League.",
    matches: [
      {
        opponent: "Argentina 1 x 0 Colômbia (Final Copa América)",
        date: "14 Jul 2024",
        competition: "Copa América (Final)",
        minutes: 120,
        finalScore: 94.0,
        receipt: {
          base: 50.0,
          actions: [
            { label: "Clean Sheet em 120 minutos", pts: "+18.0" },
            { label: "4 Defesas difíceis dentro da área", pts: "+24.0" },
            { label: "Gols prevenidos (+1.4 xGOT)", pts: "+16.8" },
            { label: "3 Saídas aéreas com firmeza", pts: "+12.0" }
          ],
          penalties: [],
          context: [
            { label: "Final de Copa América", mult: "x 1.30" }
          ]
        }
      }
    ]
  }
];

// App State
let currentPositionFilter = "all";
let currentSearchQuery = "";

// DOM Elements
const tbody = document.getElementById("leaderboard-tbody");
const searchInput = document.getElementById("player-search-input");
const filterChips = document.querySelectorAll(".filter-chip");
const navButtons = document.querySelectorAll(".nav-btn");
const tabPanes = document.querySelectorAll(".tab-pane");
const spotlightContainer = document.getElementById("top-player-spotlight");
const vsGrid = document.getElementById("vs-comparison-cards");

// Modal Elements
const modalOverlay = document.getElementById("player-modal-overlay");
const modalDrawer = document.getElementById("player-modal");
const modalContent = document.getElementById("modal-player-content");
const modalCloseBtn = document.getElementById("modal-close-btn");

// Initialize Application
document.addEventListener("DOMContentLoaded", () => {
  renderSpotlight();
  renderLeaderboard();
  renderVsSection();
  setupEventListeners();
});

// Render Hero Spotlight (Top 1: Rodri)
function renderSpotlight() {
  const top1 = PLAYERS_DATA[0];
  spotlightContainer.innerHTML = `
    <div class="spotlight-header">
      <img src="${top1.avatar}" alt="${top1.name}" class="spotlight-avatar">
      <div class="spotlight-info">
        <h3>${top1.name}</h3>
        <div class="spotlight-team">${top1.team}</div>
        <span class="spotlight-badge">${top1.posLabel}</span>
      </div>
    </div>
    
    <div class="spotlight-metrics">
      <div class="spotlight-metric-item">
        <span class="spotlight-metric-val">${top1.fssScore}</span>
        <span class="spotlight-metric-lbl">Score Sazonal</span>
      </div>
      <div class="spotlight-metric-item">
        <span class="spotlight-metric-val">${top1.mpsAvg}</span>
        <span class="spotlight-metric-lbl">Média / Jogo</span>
      </div>
      <div class="spotlight-metric-item">
        <span class="spotlight-metric-val">${top1.clutchIndex}%</span>
        <span class="spotlight-metric-lbl">Clutch</span>
      </div>
    </div>

    <div class="spotlight-quote">
      "${top1.discrepancyNote}"
    </div>
    
    <div style="margin-top: 18px; text-align: right;">
      <button class="btn-audit" onclick="openPlayerAudit('${top1.id}')">
        Auditar Partidas de ${top1.name} &rarr;
      </button>
    </div>
  `;
}

// Render Leaderboard Table
function renderLeaderboard() {
  const filtered = PLAYERS_DATA.filter(player => {
    // Position filter
    const matchesPos = currentPositionFilter === "all" || player.posCategory === currentPositionFilter;
    
    // Search query filter
    const query = currentSearchQuery.toLowerCase();
    const matchesSearch = player.name.toLowerCase().includes(query) ||
                          player.team.toLowerCase().includes(query) ||
                          player.league.toLowerCase().includes(query);

    return matchesPos && matchesSearch;
  });

  if (filtered.length === 0) {
    tbody.innerHTML = `
      <tr>
        <td colspan="8" style="text-align: center; padding: 40px; color: var(--text-muted);">
          Nenhum jogador encontrado com os filtros atuais.
        </td>
      </tr>
    `;
    return;
  }

  tbody.innerHTML = filtered.map((player) => {
    const posClass = getPosBadgeClass(player.posCategory);
    const rankClass = player.rank === 1 ? 'rank-1' : player.rank === 2 ? 'rank-2' : player.rank === 3 ? 'rank-3' : '';

    return `
      <tr class="${rankClass}" onclick="openPlayerAudit('${player.id}')">
        <td class="td-rank">#${player.rank}</td>
        <td>
          <div class="player-cell">
            <img src="${player.avatar}" alt="${player.name}" class="player-avatar-small">
            <div class="player-meta">
              <h4>${player.name}</h4>
              <div class="player-club-text">${player.team}</div>
            </div>
          </div>
        </td>
        <td>
          <span class="pos-tag ${posClass}">${player.posLabel}</span>
        </td>
        <td>
          <strong>${player.games}</strong> <span style="color: var(--text-muted); font-size: 0.78rem;">(${player.minutes}')</span>
        </td>
        <td>
          <span class="score-main">${player.mpsAvg}</span>
        </td>
        <td>
          <span style="color: var(--gold-glow); font-weight: 700;">${player.clutchIndex}%</span>
        </td>
        <td>
          <span class="score-main score-fss">${player.fssScore}</span>
        </td>
        <td>
          <button class="btn-audit" onclick="event.stopPropagation(); openPlayerAudit('${player.id}')">
            Auditar
          </button>
        </td>
      </tr>
    `;
  }).join("");
}

// Render Tab 2: Ballon d'Or vs The Real Best
function renderVsSection() {
  vsGrid.innerHTML = PLAYERS_DATA.map(p => {
    const isDiscrepancy = p.ballonRank !== p.realBestRank;
    const badgeHtml = isDiscrepancy 
      ? `<span class="vs-badge-discrepancy">Distorção Detectada</span>`
      : `<span class="vs-badge-match">Consenso Justo</span>`;

    return `
      <div class="vs-card">
        <div class="vs-card-top">
          <div class="vs-card-title">
            <h3>${p.name}</h3>
            <span style="font-size: 0.8rem; color: var(--text-muted);">${p.posLabel}</span>
          </div>
          ${badgeHtml}
        </div>

        <div class="vs-rankings-row">
          <div class="vs-rank-box">
            <div class="vs-rank-box-label">Ballon d'Or Oficial</div>
            <div class="vs-rank-box-num color-ballon">${typeof p.ballonRank === 'number' ? '#' + p.ballonRank : p.ballonRank}</div>
          </div>
          <div style="width: 1px; background: var(--border-subtle);"></div>
          <div class="vs-rank-box">
            <div class="vs-rank-box-label">The Real Best</div>
            <div class="vs-rank-box-num color-realbest">#${p.realBestRank}</div>
          </div>
        </div>

        <p class="vs-explanation">${p.discrepancyNote}</p>
        
        <div style="margin-top: 14px;">
          <button class="btn-audit" onclick="openPlayerAudit('${p.id}')">
            Ver Provas em Jogos &rarr;
          </button>
        </div>
      </div>
    `;
  }).join("");
}

// Open Player Audit Modal (Drawer)
window.openPlayerAudit = function(playerId) {
  const player = PLAYERS_DATA.find(p => p.id === playerId);
  if (!player) return;

  modalContent.innerHTML = `
    <div class="audit-profile-hero">
      <img src="${player.avatar}" alt="${player.name}" class="audit-avatar">
      <div class="audit-profile-info">
        <h2>${player.name}</h2>
        <div class="audit-profile-sub">${player.posLabel} &bull; ${player.team}</div>
      </div>
    </div>

    <div class="audit-stats-summary">
      <div class="summary-card">
        <div class="summary-card-val" style="color: var(--gold-glow);">${player.fssScore}</div>
        <div class="summary-card-lbl">Score Sazonal (FSS)</div>
      </div>
      <div class="summary-card">
        <div class="summary-card-val">${player.mpsAvg}</div>
        <div class="summary-card-lbl">Média / Partida</div>
      </div>
      <div class="summary-card">
        <div class="summary-card-val" style="color: var(--emerald-accent);">${player.clutchIndex}%</div>
        <div class="summary-card-lbl">Índice Decisivo</div>
      </div>
    </div>

    <p style="font-size: 0.88rem; color: var(--text-secondary); margin-bottom: 24px; line-height: 1.5; background: rgba(0,0,0,0.25); padding: 14px; border-radius: var(--radius-sm); border-left: 3px solid var(--gold-primary);">
      ${player.summary}
    </p>

    <div class="audit-log-title">
      <span>Extrato de Partidas Auditadas</span>
      <span class="audit-hint">Clique para abrir o recibo matemático</span>
    </div>

    <div class="match-log-list">
      ${player.matches.map((m, idx) => `
        <div class="match-item-card">
          <div class="match-item-header" onclick="toggleMatchReceipt('receipt-${player.id}-${idx}')">
            <div>
              <div class="match-teams">${m.opponent}</div>
              <div class="match-competition">${m.competition} &bull; ${m.date} &bull; ${m.minutes}' jogados</div>
            </div>
            <div class="match-score-badge">
              <span class="badge-mps">${m.finalScore} pts</span>
              <span style="font-size: 0.8rem; color: var(--text-muted);">&#9660;</span>
            </div>
          </div>

          <div class="match-receipt" id="receipt-${player.id}-${idx}">
            <div class="receipt-row">
              <span class="receipt-label">Nota Base ao Entrar em Campo:</span>
              <span class="receipt-value">+${m.receipt.base.toFixed(1)} pts</span>
            </div>

            <div class="receipt-section-title">Ações Específicas da Função</div>
            ${m.receipt.actions.map(act => `
              <div class="receipt-row">
                <span class="receipt-label">${act.label}</span>
                <span class="receipt-value pos">${act.pts}</span>
              </div>
            `).join("")}

            ${m.receipt.penalties.length > 0 ? `
              <div class="receipt-section-title">Deduções & Falhas</div>
              ${m.receipt.penalties.map(pen => `
                <div class="receipt-row">
                  <span class="receipt-label">${pen.label}</span>
                  <span class="receipt-value neg">${pen.pts}</span>
                </div>
              `).join("")}
            ` : ''}

            <div class="receipt-section-title">Multiplicadores de Contexto</div>
            ${m.receipt.context.map(ctx => `
              <div class="receipt-row">
                <span class="receipt-label">${ctx.label}</span>
                <span class="receipt-value" style="color: var(--gold-glow);">${ctx.mult}</span>
              </div>
            `).join("")}

            <div class="receipt-total-bar">
              <span class="receipt-total-lbl">Pontuação Final Auditada:</span>
              <span class="receipt-total-num">${m.finalScore} / 100 pts</span>
            </div>
          </div>
        </div>
      `).join("")}
    </div>
  `;

  modalOverlay.classList.add("open");
};

// Toggle Accordion for Match Receipt
window.toggleMatchReceipt = function(receiptId) {
  const el = document.getElementById(receiptId);
  if (el) {
    el.classList.toggle("expanded");
  }
};

// Event Listeners
function setupEventListeners() {
  // Search
  searchInput.addEventListener("input", (e) => {
    currentSearchQuery = e.target.value;
    renderLeaderboard();
  });

  // Position filter chips
  filterChips.forEach(chip => {
    chip.addEventListener("click", () => {
      filterChips.forEach(c => c.classList.remove("active"));
      chip.classList.add("active");
      currentPositionFilter = chip.dataset.pos;
      renderLeaderboard();
    });
  });

  // Nav tab switching
  navButtons.forEach(btn => {
    btn.addEventListener("click", () => {
      navButtons.forEach(b => b.classList.remove("active"));
      tabPanes.forEach(p => p.classList.remove("active"));

      btn.classList.add("active");
      const tabId = `tab-${btn.dataset.tab}`;
      const targetPane = document.getElementById(tabId);
      if (targetPane) {
        targetPane.classList.add("active");
      }
    });
  });

  // Close modal
  modalCloseBtn.addEventListener("click", () => {
    modalOverlay.classList.remove("open");
  });

  modalOverlay.addEventListener("click", (e) => {
    if (e.target === modalOverlay) {
      modalOverlay.classList.remove("open");
    }
  });

  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape") {
      modalOverlay.classList.remove("open");
    }
  });
}

function getPosBadgeClass(posCategory) {
  if (posCategory === "ST/W") return "pos-att";
  if (posCategory === "CAM") return "pos-mid";
  if (posCategory === "CM/CDM") return "pos-mid";
  if (posCategory === "CB/FB") return "pos-def";
  if (posCategory === "GK") return "pos-gk";
  return "pos-mid";
}
