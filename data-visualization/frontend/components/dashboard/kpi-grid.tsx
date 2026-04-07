import { MetricCard } from "@/components/dashboard/metric-card";
import { formatMs, formatNumber, formatPercent } from "@/lib/format";
import type { SummaryDto } from "@/types/experiments";

export function KpiGrid({ summary }: { summary: SummaryDto }) {
  return (
    <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      <MetricCard title="Total de execuções" value={formatNumber(summary.totalExecutions)} />
      <MetricCard title="Quantidade de cases" value={formatNumber(summary.totalCases)} />
      <MetricCard title="Quantidade de modelos" value={formatNumber(summary.totalModels)} />
      <MetricCard title="Latência média" value={formatMs(summary.avgExecutionTimeMs)} />
      <MetricCard title="Média de total de tokens" value={formatNumber(summary.avgTotalTokens)} />
      <MetricCard title="Taxa de seleção correta" value={formatPercent(summary.successRateSelection)} />
      <MetricCard title="Taxa de resposta correta" value={formatPercent(summary.successRateResponse)} />
      <MetricCard
        title="Taxa de uso de tool"
        value={formatPercent(summary.toolUsageRate)}
        helper={`Média de tool calls: ${formatNumber(summary.avgToolCallsCount, 2)}`}
      />
    </section>
  );
}
