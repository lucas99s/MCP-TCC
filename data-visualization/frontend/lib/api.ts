import type { ExperimentDetailDto, ExperimentFilters, ExperimentListItemDto, FilterOptionsDto, GroupedMetricDto, InsightDto, PagedResult, SummaryDto } from "@/types/experiments";

function getApiBaseUrl() {
  return process.env.API_BASE_URL ?? process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5080/api";
}

function buildQueryString(filters?: ExperimentFilters) {
  if (!filters) return "";
  const params = new URLSearchParams();
  Object.entries(filters).forEach(([key, value]) => {
    if (value === undefined || value === null) return;
    if (Array.isArray(value)) {
      value.forEach((entry) => params.append(key, entry));
      return;
    }
    params.set(key, String(value));
  });
  const raw = params.toString();
  return raw ? `?${raw}` : "";
}

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${getApiBaseUrl()}${path}`, { cache: "no-store" });
  if (!response.ok) throw new Error(`Falha ao carregar ${path}`);
  return response.json() as Promise<T>;
}

export async function fetchSummary(filters?: ExperimentFilters): Promise<SummaryDto> {
  return getJson<SummaryDto>(`/experiments/summary${buildQueryString(filters)}`);
}

export async function fetchGrouped(by: string, filters?: ExperimentFilters): Promise<GroupedMetricDto[]> {
  const qs = buildQueryString(filters);
  return getJson<GroupedMetricDto[]>(`/experiments/grouped?by=${by}${qs ? `&${qs.slice(1)}` : ""}`);
}

export async function fetchExperiments(filters?: ExperimentFilters): Promise<PagedResult<ExperimentListItemDto>> {
  return getJson<PagedResult<ExperimentListItemDto>>(`/experiments${buildQueryString(filters)}`);
}

export async function fetchExperimentDetail(id: string): Promise<ExperimentDetailDto> {
  return getJson<ExperimentDetailDto>(`/experiments/${id}`);
}

export async function fetchFilters(): Promise<FilterOptionsDto> {
  return getJson<FilterOptionsDto>(`/experiments/filters`);
}

export async function fetchInsights(filters?: ExperimentFilters): Promise<InsightDto[]> {
  return getJson<InsightDto[]>(`/experiments/insights${buildQueryString(filters)}`);
}
