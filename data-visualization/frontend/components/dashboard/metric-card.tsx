import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export function MetricCard({ title, value, helper }: { title: string; value: string; helper?: string }) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-semibold tracking-tight text-slate-950 sm:text-3xl">{value}</div>
        {helper ? <p className="mt-2 text-xs leading-5 text-slate-500 sm:text-sm">{helper}</p> : null}
      </CardContent>
    </Card>
  );
}
