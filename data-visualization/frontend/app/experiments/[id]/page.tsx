import { ExperimentDetailClient } from "@/components/dashboard/experiment-detail-client";
import { AppShell } from "@/components/layout/app-shell";

export default async function ExperimentDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  return (
    <AppShell
      title="Detalhe da execução"
      subtitle="Visão completa da execução selecionada, incluindo payloads, métricas derivadas e comparação com a média do grupo."
    >
      <ExperimentDetailClient id={id} />
    </AppShell>
  );
}
