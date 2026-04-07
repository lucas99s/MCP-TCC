import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatDate, formatMs, formatNumber } from "@/lib/format";
import type { ExperimentListItemDto } from "@/types/experiments";

export function ExperimentsTable({ rows }: { rows: ExperimentListItemDto[] }) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Tabela detalhada</CardTitle>
      </CardHeader>

      <CardContent className="hidden overflow-x-auto md:block">
        <table className="min-w-full text-sm">
          <thead className="text-left text-slate-500">
            <tr>
              <th className="pb-3">Run</th>
              <th className="pb-3">Case</th>
              <th className="pb-3">Modelo</th>
              <th className="pb-3">Prompt</th>
              <th className="pb-3">Quality</th>
              <th className="pb-3">Status</th>
              <th className="pb-3">Tool calls</th>
              <th className="pb-3">Tokens</th>
              <th className="pb-3">Latência</th>
              <th className="pb-3">Criado em</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => (
              <tr key={row.id} className="border-t border-border hover:bg-slate-50">
                <td className="py-3 font-medium">
                  <Link className="text-cyan-700 hover:underline" href={`/experiments/${row.id}`}>
                    {row.testRunId}
                  </Link>
                </td>
                <td className="py-3">{row.caseId}</td>
                <td className="py-3">{row.model}</td>
                <td className="py-3">{row.promptType}</td>
                <td className="py-3">{row.toolQuality}</td>
                <td className="py-3">{row.status ?? "-"}</td>
                <td className="py-3">{formatNumber(row.toolCallsCount)}</td>
                <td className="py-3">{formatNumber(row.totalTokens)}</td>
                <td className="py-3">{formatMs(row.executionTimeMs)}</td>
                <td className="py-3">{formatDate(row.createdRecordAt)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </CardContent>

      <CardContent className="grid gap-3 md:hidden">
        {rows.map((row) => (
          <div key={row.id} className="rounded-2xl border border-border bg-slate-50/60 p-4">
            <Link className="block break-words font-medium text-cyan-700 hover:underline" href={`/experiments/${row.id}`}>
              {row.testRunId}
            </Link>
            <div className="mt-3 grid grid-cols-2 gap-3 text-sm text-slate-600">
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Case</div>
                <div>{row.caseId}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Modelo</div>
                <div>{row.model}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Prompt</div>
                <div>{row.promptType}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Status</div>
                <div>{row.status ?? "-"}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Tokens</div>
                <div>{formatNumber(row.totalTokens)}</div>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wide text-slate-400">Latência</div>
                <div>{formatMs(row.executionTimeMs)}</div>
              </div>
            </div>
          </div>
        ))}
      </CardContent>
    </Card>
  );
}
