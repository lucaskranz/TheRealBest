# The Real Best — Beyond The Ballon

> **Futebol sem grife. Apenas mérito matemático.**

O primeiro ranking global 100% auditável de jogadores de futebol, baseado em performance jogo a jogo. Sem lobby, sem votos de compadrio, sem concurso de popularidade.

---

## Stack

| Camada | Tecnologia |
|:---|:---|
| Backend | ASP.NET Core 8 (C#) — Clean Architecture |
| Frontend | Next.js 14 (React + TypeScript) |
| Banco de Dados | PostgreSQL |
| Cache | Redis |
| i18n | next-intl (Front) + IStringLocalizer (Back) |
| Deploy | Vercel (Front) + Railway/Render (Back + DB) |

## Idiomas

🇧🇷 Português (BR) · 🇺🇸 English · 🇪🇸 Español

## Documentação

- [CONTEXT.md](./CONTEXT.md) — Fonte de verdade do projeto (LEIA PRIMEIRO)
- [docs/architecture_plan.md](./docs/architecture_plan.md) — Arquitetura detalhada
- [docs/fair_ranking_formula_specification.md](./docs/fair_ranking_formula_specification.md) — Especificação do algoritmo
- [.agents/rules/](./.agents/rules/) — Regras de código para agentes de IA

## Como Contribuir

1. Leia o CONTEXT.md para entender o projeto
2. Identifique a próxima etapa pendente no roadmap
3. Siga as regras em .agents/rules/ para a camada que for implementar
