import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatMs, formatNumber, formatPercent } from "@/lib/format";
import type { GroupedMetricDto } from "@/types/experiments";

function findGroup(rows: GroupedMetricDto[], key: string) {
  return rows.find((row) => row.groupKey.toLowerCase() === key.toLowerCase());
}

function deltaPercentage(base: number, compare: number) {
  if (base === 0) return 0;
  return ((compare - base) / base) * 100;
}

function deltaPoints(base: number, compare: number) {
  return (compare - base) * 100;
}

function ComparisonCard({
  title,
  detailedValue,
  genericValue,
  helper
}: {
  title: string;
  detailedValue: string;
  genericValue: string;
  helper: string;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid gap-3 text-sm text-slate-600">
          <div className="rounded-2xl bg-slate-50 p-4">
            <div className="text-xs font-semibold uppercase tracking-[0.14em] text-slate-400">Detailed</div>
            <div className="mt-2 text-2xl font-semibold text-slate-950">{detailedValue}</div>
          </div>
          <div className="rounded-2xl bg-slate-50 p-4">
            <div className="text-xs font-semibold uppercase tracking-[0.14em] text-slate-400">Generic</div>
            <div className="mt-2 text-2xl font-semibold text-slate-950">{genericValue}</div>
          </div>
          <p className="text-sm text-slate-500">{helper}</p>
        </div>
      </CardContent>
    </Card>
  );
}

export function ToolQualityFocus({ rows }: { rows: GroupedMetricDto[] }) {
  const detailed = findGroup(rows, "detailed");
  const generic = findGroup(rows, "generic");

  if (!detailed && !generic) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Análise da descrição das tools</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-slate-600">Ainda não há dados agrupados por `tool_quality` no recorte atual.</p>
        </CardContent>
      </Card>
    );
  }

  const detailedSelection = detailed?.successRateSelection ?? 0;
  const genericSelection = generic?.successRateSelection ?? 0;
  const detailedResponse = detailed?.successRateResponse ?? 0;
  const genericResponse = generic?.successRateResponse ?? 0;
  const detailedUsage = detailed?.toolUsageRate ?? 0;
  const genericUsage = generic?.toolUsageRate ?? 0;
  const detailedTokens = detailed?.avgTotalTokens ?? 0;
  const genericTokens = generic?.avgTotalTokens ?? 0;
  const detailedLatency = detailed?.avgExecutionTimeMs ?? 0;
  const genericLatency = generic?.avgExecutionTimeMs ?? 0;

  return (
    <section className="space-y-6">
      <div>
        <h2 className="text-xl font-semibold tracking-tight text-slate-950">Impacto da descrição das tools</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">
          Esta seção destaca o efeito das descrições <b>genéricas</b> vs <b>detalhadas</b> sobre precisão, uso de tools, custo
          computacional e resposta do modelo.
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <ComparisonCard
          title="Acerto de seleção"
          detailedValue={formatPercent(detailedSelection)}
          genericValue={formatPercent(genericSelection)}
          helper={`Diferença de ${formatNumber(deltaPoints(genericSelection, detailedSelection), 1)} p.p. em favor de ${detailedSelection >= genericSelection ? "detalhadas" : "genéricas"}.`}
        />
        <ComparisonCard
          title="Acerto de resposta"
          detailedValue={formatPercent(detailedResponse)}
          genericValue={formatPercent(genericResponse)}
          helper={`Diferença de ${formatNumber(deltaPoints(genericResponse, detailedResponse), 1)} p.p. em favor de ${detailedResponse >= genericResponse ? "detalhadas" : "genéricas"}.`}
        />
        <ComparisonCard
          title="Uso de tools"
          detailedValue={formatPercent(detailedUsage)}
          genericValue={formatPercent(genericUsage)}
          helper={`Diferença de ${formatNumber(deltaPoints(genericUsage, detailedUsage), 1)} p.p. em favor de ${detailedUsage >= genericUsage ? "detalhadas" : "genéricas"}.`}
        />
        <ComparisonCard
          title="Custo médio"
          detailedValue={formatNumber(detailedTokens)}
          genericValue={formatNumber(genericTokens)}
          helper={`Tools detalhadas consumiram ${formatNumber(Math.abs(deltaPercentage(genericTokens, detailedTokens)), 1)}% ${detailedTokens <= genericTokens ? "menos" : "mais"} tokens que genéricas.`}
        />
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Leitura analítica</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-3 text-sm text-slate-600">
          <p>
            Descrições <b>detalhadas</b> tiveram <strong>{formatPercent(detailedSelection)}</strong> de acerto de seleção e{" "}
            <strong>{formatPercent(detailedResponse)}</strong> de acerto de resposta.
          </p>
          <p>
            Descrições <b>genéricas</b> tiveram <strong>{formatPercent(genericSelection)}</strong> de acerto de seleção e{" "}
            <strong>{formatPercent(genericResponse)}</strong> de acerto de resposta.
          </p>
          <p>
            Em uso efetivo de ferramentas, tools <b>detalhadas</b> marcaram <strong>{formatPercent(detailedUsage)}</strong> contra{" "}
            <strong>{formatPercent(genericUsage)}</strong> em <b>genéricas</b>.
          </p>
          <p>
            Em custo, tools <b>detalhadas</b> ficaram em média em <strong>{formatNumber(detailedTokens)}</strong> tokens e <b>genéricas</b> em{" "}
            <strong>{formatNumber(genericTokens)}</strong>.
          </p>
          <p>
            Em latência média, tools <b>detalhadas</b> ficaram em <strong>{formatMs(detailedLatency)}</strong> e <b>genéricas</b> em{" "}
            <strong>{formatMs(genericLatency)}</strong>.
          </p>
        </CardContent>
      </Card>
    </section>
  );
}
