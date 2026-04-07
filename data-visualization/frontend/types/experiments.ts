export type Tone = "neutral" | "positive" | "warning" | "critical";

export interface ExperimentFilters {
  startDate?: string;
  endDate?: string;
  caseIds?: string[];
  models?: string[];
  promptTypes?: string[];
  toolQualities?: string[];
  statuses?: string[];
  executionModes?: string[];
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortDirection?: "asc" | "desc";
}

export interface SummaryDto {
  totalExecutions: number;
  totalCases: number;
  totalModels: number;
  avgExecutionTimeMs: number;
  avgLatencySeconds: number;
  avgTotalTokens: number;
  avgReasoningTokens: number;
  successRateSelection: number;
  successRateResponse: number;
  toolUsageRate: number;
  avgToolCallsCount: number;
  nullSelectedToolRate: number;
  failureRate: number;
  rowsWithToolExpectedButNotUsed: number;
}

export interface GroupedMetricDto {
  groupKey: string;
  executions: number;
  avgExecutionTimeMs: number;
  medianExecutionTimeMs: number;
  avgTotalTokens: number;
  medianTotalTokens: number;
  avgReasoningTokens: number;
  successRateSelection: number;
  successRateResponse: number;
  toolUsageRate: number;
  nullSelectedToolRate: number;
  avgToolCallsCount: number;
}

export interface ExperimentListItemDto {
  id: number;
  caseId: string;
  testRunId?: string | null;
  model: string;
  promptType: string;
  runNumber: number;
  toolQuality: string;
  executionMode?: string | null;
  status?: string | null;
  toolChoice?: string | null;
  selectedTool?: string | null;
  toolCallsCount?: number | null;
  selectionCorrect?: boolean | null;
  responseCorrect?: boolean | null;
  tokensInput?: number | null;
  tokensOutput?: number | null;
  totalTokens?: number | null;
  reasoningTokens?: number | null;
  executionTimeMs?: number | null;
  createdRecordAt?: string | null;
  toolUsed: boolean;
  avgLatencySeconds?: number | null;
  outputInputRatio?: number | null;
  reasoningShare?: number | null;
}

export interface ExperimentBaselineDto {
  groupLabel: string;
  avgExecutionTimeMs: number;
  avgTotalTokens: number;
  avgReasoningTokens: number;
  successRateSelection: number;
  successRateResponse: number;
}

export interface ExperimentDetailDto extends ExperimentListItemDto {
  promptText: string;
  expectedTool?: string | null;
  rawResponse?: string | null;
  notes?: string | null;
  responseId?: string | null;
  executionId?: string | null;
  modelReturned?: string | null;
  toolsAvailableCount?: number | null;
  startedAtMs?: number | null;
  completedAt?: number | null;
  tokenEfficiency?: number | null;
  expectedToolNotUsed: boolean;
  groupBaseline?: ExperimentBaselineDto | null;
}

export interface FilterOptionsDto {
  caseIds: string[];
  models: string[];
  promptTypes: string[];
  toolQualities: string[];
  statuses: string[];
  executionModes: string[];
}

export interface InsightDto {
  id: string;
  title: string;
  description: string;
  tone: Tone;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
