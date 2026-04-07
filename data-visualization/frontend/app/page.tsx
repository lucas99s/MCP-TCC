import { DashboardClient } from "@/components/dashboard/dashboard-client";
import { AppShell } from "@/components/layout/app-shell";

export default function DashboardPage() {
  return (
    <AppShell
      title="Dashboard de experimentos MCP"
      subtitle="Interface analítica para explorar custo, performance, uso de tools e indicadores de acerto em fluxos de avaliação com LLMs."
    >
      <DashboardClient />
    </AppShell>
  );
}
