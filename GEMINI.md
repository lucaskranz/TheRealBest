# GEMINI.md — Regra Obrigatória do Projeto The Real Best

## INSTRUÇÃO PRIMÁRIA (OBRIGATÓRIA)
Antes de realizar QUALQUER trabalho neste repositório (código, documentação, análise, debug), você DEVE:

1. **Ler o arquivo CONTEXT.md na raiz do repositório.** Ele é a fonte de verdade do projeto e contém:
   - A stack tecnológica completa
   - A arquitetura do backend e frontend
   - O modelo de dados
   - O algoritmo de pontuação
   - O roadmap de implementação com status de cada etapa
   - As convenções de código e commits

2. **Identificar a etapa atual do roadmap.** Verifique no CONTEXT.md qual é a próxima etapa com status ⬜ Pendente antes de agir.

3. **Ler as Rules específicas da camada** que você vai modificar (em .agents/rules/):
   - ackend-rules.md para trabalho em C# / .NET
   - rontend-rules.md para trabalho em Next.js / React / TypeScript
   - scoring-rules.md para trabalho no motor de pontuação
   - database-rules.md para trabalho em banco de dados / EF Core

4. **Ao concluir uma etapa**, atualize o status no CONTEXT.md de ⬜ Pendente para ✅ Concluída.

## REGRAS GERAIS DO PROJETO
- Idiomas suportados: Português (BR), English, Español — NUNCA hardcodar strings de UI
- Backend: ASP.NET Core 8 com Clean Architecture rigorosa
- Frontend: Next.js 14 com App Router e next-intl para i18n
- Banco: PostgreSQL com EF Core 8 (Code First, Fluent API)
- Commits: formato 	ipo(escopo): descrição (ex: eat(scoring): add striker weight matrix)
- Testes: obrigatórios para o Scoring Engine, desejáveis para demais camadas

## REFERÊNCIA RÁPIDA DE DOCUMENTAÇÃO
- CONTEXT.md — Fonte de verdade (stack, roadmap, modelo de dados, convenções)
- docs/architecture_plan.md — Arquitetura detalhada com diagramas
- docs/fair_ranking_formula_specification.md — Especificação matemática completa do algoritmo
