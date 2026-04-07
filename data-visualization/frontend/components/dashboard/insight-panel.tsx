import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { InsightDto } from "@/types/experiments";

export function InsightPanel({ insights }: { insights: InsightDto[] }) {
  return (
    <Card>
      <CardHeader>
        <div>
          <CardTitle>Insights automáticos</CardTitle>
          <p className="mt-1 text-sm text-slate-500">Leituras calculadas dinamicamente com base no filtro atual.</p>
        </div>
      </CardHeader>
      <CardContent className="grid gap-3">
        {insights.length === 0 ? (
          <div className="rounded-2xl border border-dashed border-border p-4 text-sm text-slate-500">
            Ainda não há insight forte o suficiente para destacar.
          </div>
        ) : (
          insights.map((insight) => (
            <div key={insight.id} className="rounded-2xl border border-border bg-slate-50/70 p-4">
              <div className="flex items-center gap-3">
                <Badge tone={insight.tone}>{insight.tone}</Badge>
                <h4 className="font-medium text-slate-900">{insight.title}</h4>
              </div>
              <p className="mt-2 text-sm text-slate-600">{insight.description}</p>
            </div>
          ))
        )}
      </CardContent>
    </Card>
  );
}
