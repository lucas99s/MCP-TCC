"use client";

import { useQuery } from "@tanstack/react-query";
import { ExecutionDetailPanel } from "@/components/dashboard/execution-detail-panel";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { fetchExperimentDetail } from "@/lib/api";

export function ExperimentDetailClient({ id }: { id: string }) {
  const detailQuery = useQuery({
    queryKey: ["experiment-detail", id],
    queryFn: () => fetchExperimentDetail(id)
  });

  if (detailQuery.isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Carregando execução</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="h-40 animate-pulse rounded-2xl bg-slate-100" />
        </CardContent>
      </Card>
    );
  }

  if (detailQuery.error || !detailQuery.data) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Falha ao carregar detalhe</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-slate-600">
            {detailQuery.error instanceof Error ? detailQuery.error.message : "Não foi possível carregar o detalhe da execução."}
          </p>
        </CardContent>
      </Card>
    );
  }

  return <ExecutionDetailPanel detail={detailQuery.data} />;
}
