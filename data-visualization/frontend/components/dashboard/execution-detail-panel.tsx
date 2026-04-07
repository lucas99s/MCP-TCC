import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatMs, formatNumber, formatPercent } from "@/lib/format";
import type { ExperimentDetailDto } from "@/types/experiments";

export function ExecutionDetailPanel({ detail }: { detail: ExperimentDetailDto }) {
  return (
    <div className="grid gap-6 xl:grid-cols-[1.1fr_0.9fr]">
      <Card>
        <CardHeader>
          <div>
            <CardTitle>{detail.testRunId}</CardTitle>
            <p className="mt-1 text-sm text-slate-500">Metadados completos e payloads da execução.</p>
          </div>
        </CardHeader>
        <CardContent className="grid gap-6">
          <div className="grid gap-3 text-sm leading-6 md:grid-cols-2">
            <div><strong>Case:</strong> {detail.caseId}</div>
            <div><strong>Modelo:</strong> {detail.model}</div>
            <div><strong>Prompt type:</strong> {detail.promptType}</div>
            <div><strong>Tool quality:</strong> {detail.toolQuality}</div>
            <div><strong>Status:</strong> {detail.status}</div>
            <div><strong>Latência:</strong> {formatMs(detail.executionTimeMs)}</div>
            <div><strong>Total tokens:</strong> {formatNumber(detail.totalTokens)}</div>
            <div><strong>Reasoning share:</strong> {formatPercent(detail.reasoningShare)}</div>
          </div>
          <div>
            <h4 className="mb-2 font-medium text-slate-900">Prompt</h4>
            <pre className="overflow-x-auto whitespace-pre-wrap break-words rounded-2xl bg-slate-950 p-4 text-xs leading-6 text-slate-100">{detail.promptText}</pre>
          </div>
          <div>
            <h4 className="mb-2 font-medium text-slate-900">Expected tool</h4>
            <pre className="overflow-x-auto whitespace-pre-wrap break-words rounded-2xl bg-slate-100 p-4 text-xs leading-6 text-slate-700">{detail.expectedTool ?? "null"}</pre>
          </div>
          <div>
            <h4 className="mb-2 font-medium text-slate-900">Raw response</h4>
            <pre className="overflow-x-auto whitespace-pre-wrap break-words rounded-2xl bg-slate-950 p-4 text-xs leading-6 text-slate-100">{detail.rawResponse ?? "Sem conteúdo"}</pre>
          </div>
        </CardContent>
      </Card>

      <div className="grid gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Indicadores visuais</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-3">
            <div className="flex flex-wrap gap-3">
              <Badge tone={detail.selectionCorrect ? "positive" : "critical"}>Seleção {detail.selectionCorrect ? "correta" : "incorreta"}</Badge>
              <Badge tone={detail.responseCorrect ? "positive" : "critical"}>Resposta {detail.responseCorrect ? "correta" : "incorreta"}</Badge>
            </div>
            <div className="rounded-2xl bg-slate-50 p-4 text-sm text-slate-600">
              Token efficiency: <strong>{formatNumber(detail.tokenEfficiency, 4)}</strong>
            </div>
            <div className="rounded-2xl bg-slate-50 p-4 text-sm text-slate-600">
              Expected tool não utilizada: <strong>{detail.expectedToolNotUsed ? "sim" : "não"}</strong>
            </div>
          </CardContent>
        </Card>

        {detail.groupBaseline ? (
          <Card>
            <CardHeader>
              <CardTitle>Comparação com a média do grupo</CardTitle>
            </CardHeader>
            <CardContent className="grid gap-3 text-sm text-slate-600">
              <div><strong>Grupo:</strong> {detail.groupBaseline.groupLabel}</div>
              <div><strong>Latência média:</strong> {formatMs(detail.groupBaseline.avgExecutionTimeMs)}</div>
              <div><strong>Total tokens médio:</strong> {formatNumber(detail.groupBaseline.avgTotalTokens)}</div>
              <div><strong>Reasoning médio:</strong> {formatNumber(detail.groupBaseline.avgReasoningTokens)}</div>
              <div><strong>Acerto seleção:</strong> {formatPercent(detail.groupBaseline.successRateSelection)}</div>
              <div><strong>Acerto resposta:</strong> {formatPercent(detail.groupBaseline.successRateResponse)}</div>
            </CardContent>
          </Card>
        ) : null}
      </div>
    </div>
  );
}
