"use client";

import { startTransition, useDeferredValue, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { BarChartCard } from "@/components/charts/bar-chart-card";
import { HeatmapCard } from "@/components/charts/heatmap-card";
import { ScatterChartCard } from "@/components/charts/scatter-chart-card";
import { FilterBar } from "@/components/dashboard/filter-bar";
import { GroupedMetricsTable } from "@/components/dashboard/grouped-metrics-table";
import { InsightPanel } from "@/components/dashboard/insight-panel";
import { KpiGrid } from "@/components/dashboard/kpi-grid";
import { ToolQualityFocus } from "@/components/dashboard/tool-quality-focus";
import { ExperimentsTable } from "@/components/table/experiments-table";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { fetchExperiments, fetchFilters, fetchGrouped, fetchInsights, fetchSummary } from "@/lib/api";
import type { ExperimentFilters } from "@/types/experiments";

function LoadingPanel() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Carregando dashboard</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="h-28 animate-pulse rounded-2xl bg-slate-100" />
      </CardContent>
    </Card>
  );
}

function ErrorPanel({ message }: { message: string }) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Falha ao carregar dados</CardTitle>
      </CardHeader>
      <CardContent>
        <p className="text-sm text-slate-600">{message}</p>
      </CardContent>
    </Card>
  );
}

function createDefaultFilters(): ExperimentFilters {
  return {
    page: 1,
    pageSize: 20,
    sortBy: "created_record_at",
    sortDirection: "desc"
  };
}

export function DashboardClient() {
  const [filters, setFilters] = useState<ExperimentFilters>(createDefaultFilters);
  const deferredFilters = useDeferredValue(filters);

  function handleFilterChange(patch: Partial<ExperimentFilters>) {
    startTransition(() => {
      setFilters((current) => ({
        ...current,
        ...patch,
        page: 1
      }));
    });
  }

  function handleResetFilters() {
    startTransition(() => {
      setFilters(createDefaultFilters());
    });
  }

  const summaryQuery = useQuery({
    queryKey: ["summary", deferredFilters],
    queryFn: () => fetchSummary(deferredFilters)
  });

  const groupedPromptTypeQuery = useQuery({
    queryKey: ["grouped", "prompt_type", deferredFilters],
    queryFn: () => fetchGrouped("prompt_type", deferredFilters)
  });

  const groupedToolQualityQuery = useQuery({
    queryKey: ["grouped", "tool_quality", deferredFilters],
    queryFn: () => fetchGrouped("tool_quality", deferredFilters)
  });

  const experimentsQuery = useQuery({
    queryKey: ["experiments", deferredFilters],
    queryFn: () => fetchExperiments(deferredFilters)
  });

  const filtersQuery = useQuery({
    queryKey: ["filters"],
    queryFn: fetchFilters
  });

  const insightsQuery = useQuery({
    queryKey: ["insights", deferredFilters],
    queryFn: () => fetchInsights(deferredFilters)
  });

  const isLoading =
    summaryQuery.isLoading ||
    groupedPromptTypeQuery.isLoading ||
    groupedToolQualityQuery.isLoading ||
    experimentsQuery.isLoading ||
    filtersQuery.isLoading ||
    insightsQuery.isLoading;

  const error =
    summaryQuery.error ||
    groupedPromptTypeQuery.error ||
    groupedToolQualityQuery.error ||
    experimentsQuery.error ||
    filtersQuery.error ||
    insightsQuery.error;

  if (isLoading) {
    return (
      <div className="space-y-6">
        <LoadingPanel />
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {Array.from({ length: 4 }).map((_, index) => (
            <div key={index} className="h-36 animate-pulse rounded-3xl bg-white/70 shadow-panel" />
          ))}
        </div>
      </div>
    );
  }

  if (
    error ||
    !summaryQuery.data ||
    !groupedPromptTypeQuery.data ||
    !groupedToolQualityQuery.data ||
    !experimentsQuery.data ||
    !filtersQuery.data ||
    !insightsQuery.data
  ) {
    return <ErrorPanel message={error instanceof Error ? error.message : "Não foi possível carregar os dados do dashboard."} />;
  }

  const summary = summaryQuery.data;
  const groupedByPromptType = groupedPromptTypeQuery.data;
  const groupedByToolQuality = groupedToolQualityQuery.data;
  const experiments = experimentsQuery.data;
  const filterOptions = filtersQuery.data;
  const insights = insightsQuery.data;

  return (
    <>
      <FilterBar filters={filters} onChange={handleFilterChange} onReset={handleResetFilters} options={filterOptions} />
      <KpiGrid summary={summary} />
      <InsightPanel insights={insights} />

      <section className="grid gap-6 lg:grid-cols-2 xl:grid-cols-3">
        <BarChartCard title="Latência média por prompt_type" data={groupedByPromptType} metric="avgExecutionTimeMs" />
        <BarChartCard title="Total de tokens por prompt_type" data={groupedByPromptType} metric="avgTotalTokens" color="#0891b2" />
        <BarChartCard title="Reasoning tokens por prompt_type" data={groupedByPromptType} metric="avgReasoningTokens" color="#16a34a" />
      </section>

      <ToolQualityFocus rows={groupedByToolQuality} />

      <section className="grid gap-6 lg:grid-cols-2 xl:grid-cols-3">
        <BarChartCard title="Latência média por descrição da tool" data={groupedByToolQuality} metric="avgExecutionTimeMs" color="#dc2626" />
        <BarChartCard title="Tokens médios por descrição da tool" data={groupedByToolQuality} metric="avgTotalTokens" color="#7c3aed" />
        <BarChartCard title="Uso de tools por descrição" data={groupedByToolQuality} metric="toolUsageRate" color="#0f766e" />
      </section>

      <section className="grid gap-6 xl:grid-cols-2">
        <ScatterChartCard title="Execution time vs total tokens" items={experiments.items} xKey="executionTimeMs" yKey="totalTokens" />
        <ScatterChartCard title="Reasoning tokens vs execution time" items={experiments.items} xKey="reasoningTokens" yKey="executionTimeMs" />
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
        <GroupedMetricsTable rows={groupedByPromptType} title="Comparação por tipo de prompt" />
        <HeatmapCard items={experiments.items} />
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.2fr_0.8fr]">
        <GroupedMetricsTable rows={groupedByToolQuality} title="Comparação por descrição da tool" />
        <Card>
          <CardHeader>
            <CardTitle>Leituras orientadas ao tipo de tool</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm leading-6 text-slate-600">
            <p>
              Este recorte ajuda a comparar se descrições <strong>generic</strong> e <strong>detailed</strong> alteram a chance de a IA
              selecionar a ferramenta esperada.
            </p>
            <p>
              O foco principal aqui é observar o equilíbrio entre <strong>precisão</strong>, <strong>uso efetivo de tools</strong> e{" "}
              <strong>custo</strong> em tokens e latência.
            </p>
            <p>
              Quando houver mais volume, esta seção passa a mostrar de forma mais confiável se descrições detalhadas melhoram a resposta
              do modelo ou apenas aumentam o custo operacional.
            </p>
          </CardContent>
        </Card>
      </section>

      <ExperimentsTable rows={experiments.items} />
    </>
  );
}
