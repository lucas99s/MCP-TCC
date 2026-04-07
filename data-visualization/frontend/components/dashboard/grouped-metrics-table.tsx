import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatMs, formatNumber, formatPercent } from "@/lib/format";
import type { GroupedMetricDto } from "@/types/experiments";

export function GroupedMetricsTable({
  rows,
  title = "Comparação por dimensão"
}: {
  rows: GroupedMetricDto[];
  title?: string;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
      </CardHeader>

      <CardContent className="hidden overflow-x-auto md:block">
        <table className="min-w-full text-sm">
          <thead className="text-left text-slate-500">
            <tr>
              <th className="pb-3">Grupo</th>
              <th className="pb-3">Execuções</th>
              <th className="pb-3">Latência média</th>
              <th className="pb-3">Tokens médios</th>
              <th className="pb-3">Reasoning médio</th>
              <th className="pb-3">Acerto seleção</th>
              <th className="pb-3">Acerto resposta</th>
              <th className="pb-3">Uso de tools</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => (
              <tr key={row.groupKey} className="border-t border-border">
                <td className="py-3 font-medium text-slate-900">{row.groupKey}</td>
                <td className="py-3">{formatNumber(row.executions)}</td>
                <td className="py-3">{formatMs(row.avgExecutionTimeMs)}</td>
                <td className="py-3">{formatNumber(row.avgTotalTokens)}</td>
                <td className="py-3">{formatNumber(row.avgReasoningTokens)}</td>
                <td className="py-3">{formatPercent(row.successRateSelection)}</td>
                <td className="py-3">{formatPercent(row.successRateResponse)}</td>
                <td className="py-3">{formatPercent(row.toolUsageRate)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </CardContent>

      <CardContent className="grid gap-3 md:hidden">
        {rows.map((row) => (
          <div key={row.groupKey} className="rounded-2xl border border-border bg-slate-50/60 p-4">
            <div className="font-medium text-slate-900">{row.groupKey}</div>
            <div className="mt-3 grid grid-cols-2 gap-3 text-sm text-slate-600">
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Execuções</div>
                <div>{formatNumber(row.executions)}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Latência</div>
                <div>{formatMs(row.avgExecutionTimeMs)}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Tokens</div>
                <div>{formatNumber(row.avgTotalTokens)}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Reasoning</div>
                <div>{formatNumber(row.avgReasoningTokens)}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Seleção</div>
                <div>{formatPercent(row.successRateSelection)}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Resposta</div>
                <div>{formatPercent(row.successRateResponse)}</div>
              </div>
            </div>
          </div>
        ))}
      </CardContent>
    </Card>
  );
}
