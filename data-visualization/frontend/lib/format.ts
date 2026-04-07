export function formatNumber(value?: number | null, fractionDigits = 0) {
  if (value === null || value === undefined || Number.isNaN(value)) return "-";
  return new Intl.NumberFormat("pt-BR", {
    minimumFractionDigits: fractionDigits,
    maximumFractionDigits: fractionDigits
  }).format(value);
}

export function formatPercent(value?: number | null, fractionDigits = 1) {
  if (value === null || value === undefined || Number.isNaN(value)) return "-";
  return `${formatNumber(value * 100, fractionDigits)}%`;
}

export function formatMs(value?: number | null) {
  if (value === null || value === undefined) return "-";
  if (value >= 1000) return `${formatNumber(value / 1000, 1)} s`;
  return `${formatNumber(value)} ms`;
}

export function formatDate(value?: string | null) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("pt-BR", {
    dateStyle: "short",
    timeStyle: "short"
  }).format(new Date(value));
}
