# Arquitetura proposta

## Visão geral

O sistema foi dividido em duas camadas:

1. `backend/src/Api`
   API analítica em ASP.NET Core para consulta, agregação, insights e paginação server-side.

2. `frontend`
   Aplicação Next.js responsável por experiência analítica, gráficos, tabela detalhada e navegação de detalhe.

## Decisões técnicas

- Backend em `.NET 9 + Minimal API + Dapper + Npgsql`
  Motivo: simplicidade, performance em leitura analítica e facilidade para evoluir queries agregadas.

- Frontend em `Next.js + React + TypeScript + Tailwind + ECharts`
  Motivo: excelente base para dashboards modernos, SSR quando útil e boa ergonomia para evolução futura.

- Paginação, filtros e agregações no backend
  Motivo: evita transportar `raw_response` e dados pesados em listagens, além de escalar melhor.

- Métricas derivadas calculadas no backend
  Motivo: mantém consistência entre cards, gráficos, insights e exportações.

## Contratos da API

- `GET /api/experiments`
  Lista paginada com filtros e ordenação.

- `GET /api/experiments/summary`
  KPIs consolidados do recorte atual.

- `GET /api/experiments/grouped?by=prompt_type`
  Agregações por dimensão analítica.

- `GET /api/experiments/{id}`
  Detalhe completo da execução.

- `GET /api/experiments/filters`
  Valores distintos para filtros globais.

- `GET /api/experiments/insights`
  Frases analíticas automáticas para o recorte atual.

## Métricas calculadas no backend

- `tool_usage_rate`
- `null_selected_tool_rate`
- `success_rate_selection`
- `success_rate_response`
- `failure_rate`
- `rows_with_tool_expected_but_not_used`
- `avg_latency_seconds`
- `output_input_ratio`
- `reasoning_share`
- `token_efficiency`

## Definição adotada para token_efficiency

Foi adotada uma definição simples e extensível:

- se `response_correct = true`, `token_efficiency = 1000 / total_tokens`
- se `response_correct = false`, `token_efficiency = 0`

Isso torna o indicador comparável entre execuções corretas com custos diferentes. No futuro, ele pode evoluir para uma fórmula ponderada por `selection_correct`, uso de tool e latência.

## Estrutura de telas

### Dashboard executivo

- KPIs
- insights automáticos
- gráficos principais
- comparação por grupo
- tabela detalhada

### Detalhe da execução

- metadados
- prompt
- expected tool em JSON
- raw response
- indicadores de sucesso
- comparação com baseline do grupo