"use client";

import ReactECharts from "echarts-for-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { ExperimentListItemDto } from "@/types/experiments";

export function HeatmapCard({ items }: { items: ExperimentListItemDto[] }) {
  const models = [...new Set(items.map((item) => item.model))];
  const promptTypes = [...new Set(items.map((item) => item.promptType))];
  const matrix = promptTypes.flatMap((prompt, y) =>
    models.map((model, x) => {
      const subset = items.filter((item) => item.model === model && item.promptType === prompt);
      const avg = subset.length === 0 ? 0 : subset.reduce((sum, item) => sum + (item.totalTokens ?? 0), 0) / subset.length;
      return [x, y, Number(avg.toFixed(2))];
    })
  );

  return (
    <Card>
      <CardHeader>
        <CardTitle>Heatmap model x prompt_type</CardTitle>
      </CardHeader>
      <CardContent className="px-3 pb-3 sm:px-6 sm:pb-6">
        <ReactECharts
          style={{ height: 240 }}
          option={{
            tooltip: {},
            grid: { left: 20, right: 20, top: 20, bottom: 60, containLabel: true },
            xAxis: { type: "category", data: models, axisLabel: { fontSize: 11 } },
            yAxis: { type: "category", data: promptTypes, axisLabel: { fontSize: 11 } },
            visualMap: {
              min: 0,
              max: Math.max(...matrix.map((item) => item[2] as number), 1),
              orient: "horizontal",
              left: "center",
              bottom: 0,
              itemWidth: 14,
              textStyle: { fontSize: 11 }
            },
            series: [{ type: "heatmap", data: matrix, label: { show: true } }]
          }}
        />
      </CardContent>
    </Card>
  );
}
