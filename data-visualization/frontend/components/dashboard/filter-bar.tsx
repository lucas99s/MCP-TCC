import type { ReactNode } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import type { ExperimentFilters, FilterOptionsDto } from "@/types/experiments";

function FilterField({
  label,
  children,
  className = ""
}: {
  label: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <div className={`space-y-2 ${className}`}>
      <label className="block text-xs font-semibold uppercase tracking-[0.14em] text-slate-500">{label}</label>
      {children}
    </div>
  );
}

export function FilterBar({
  filters,
  options,
  onChange,
  onReset
}: {
  filters: ExperimentFilters;
  options: FilterOptionsDto;
  onChange: (patch: Partial<ExperimentFilters>) => void;
  onReset: () => void;
}) {
  return (
    <Card>
      <CardHeader>
        <div className="w-full">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
            <div>
              <CardTitle>Filtros globais</CardTitle>
              <p className="mt-1 text-sm text-slate-500">Todos os cards, gráficos e tabelas respondem ao recorte atual.</p>
            </div>
            <button
              className="inline-flex h-10 items-center justify-center rounded-2xl border border-border bg-white px-4 text-sm font-medium text-slate-700 transition-colors hover:bg-slate-50"
              onClick={onReset}
              type="button"
            >
              Limpar filtros
            </button>
          </div>
        </div>
      </CardHeader>

      <CardContent className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <FilterField className="sm:col-span-2 xl:col-span-4" label="Busca">
          <Input
            onChange={(event) => onChange({ search: event.target.value || undefined })}
            placeholder="Buscar por case, run, prompt ou tool..."
            value={filters.search ?? ""}
          />
        </FilterField>

        <FilterField label="Data inicial">
          <Input
            onChange={(event) => onChange({ startDate: event.target.value || undefined })}
            type="date"
            value={filters.startDate ?? ""}
          />
        </FilterField>

        <FilterField label="Data final">
          <Input
            onChange={(event) => onChange({ endDate: event.target.value || undefined })}
            type="date"
            value={filters.endDate ?? ""}
          />
        </FilterField>

        <FilterField label="Case">
          <select
            className="h-10 w-full rounded-2xl border border-border bg-white px-3 text-sm"
            onChange={(event) => onChange({ caseIds: event.target.value ? [event.target.value] : undefined })}
            value={filters.caseIds?.[0] ?? ""}
          >
            <option value="">Todos os cases</option>
            {options.caseIds.map((item) => (
              <option key={item} value={item}>
                {item}
              </option>
            ))}
          </select>
        </FilterField>

        <FilterField label="Modelo">
          <select
            className="h-10 w-full rounded-2xl border border-border bg-white px-3 text-sm"
            onChange={(event) => onChange({ models: event.target.value ? [event.target.value] : undefined })}
            value={filters.models?.[0] ?? ""}
          >
            <option value="">Todos os modelos</option>
            {options.models.map((item) => (
              <option key={item} value={item}>
                {item}
              </option>
            ))}
          </select>
        </FilterField>

        <FilterField label="Tipo de prompt">
          <select
            className="h-10 w-full rounded-2xl border border-border bg-white px-3 text-sm"
            onChange={(event) => onChange({ promptTypes: event.target.value ? [event.target.value] : undefined })}
            value={filters.promptTypes?.[0] ?? ""}
          >
            <option value="">Todos os tipos de prompt</option>
            {options.promptTypes.map((item) => (
              <option key={item} value={item}>
                {item}
              </option>
            ))}
          </select>
        </FilterField>

        <FilterField label="Descrição da tool">
          <select
            className="h-10 w-full rounded-2xl border border-border bg-white px-3 text-sm"
            onChange={(event) => onChange({ toolQualities: event.target.value ? [event.target.value] : undefined })}
            value={filters.toolQualities?.[0] ?? ""}
          >
            <option value="">Todos os tipos de descrição</option>
            {options.toolQualities.map((item) => (
              <option key={item} value={item}>
                {item}
              </option>
            ))}
          </select>
        </FilterField>
      </CardContent>
    </Card>
  );
}
