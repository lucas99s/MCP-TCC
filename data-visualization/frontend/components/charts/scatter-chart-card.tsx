"use client";

import ReactECharts from "echarts-for-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { ExperimentListItemDto } from "@/types/experiments";

export function ScatterChartCard({
  title,
  items,
  xKey,
  yKey
}: {
  title: string;
  items: ExperimentListItemDto[];
  xKey: keyof ExperimentListItemDto;
  yKey: keyof ExperimentListItemDto;
}) {
  const points = items
    .filter((item) => typeof item[xKey] === "number" && typeof item[yKey] === "number")
    .map((item) => [item[xKey] as number, item[yKey] as number, item.promptType, item.testRunId]);

  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
      </CardHeader>
      <CardContent className="px-3 pb-3 sm:px-6 sm:pb-6">
        <ReactECharts
          style={{ height: 260 }}
          option={{
            grid: { left: 26, right: 16, top: 24, bottom: 50, containLabel: true },
            tooltip: {
              formatter: (params: { data: [number, number, string, string] }) =>
                `${params.data[3]}<br/>${String(xKey)}: ${params.data[0]}<br/>${String(yKey)}: ${params.data[1]}`
            },
            xAxis: { type: "value", name: String(xKey), nameGap: 18, axisLabel: { fontSize: 11 } },
            yAxis: { type: "value", name: String(yKey), nameGap: 18, axisLabel: { fontSize: 11 } },
            series: [{ type: "scatter", symbolSize: 18, itemStyle: { color: "#0891b2" }, data: points }]
          }}
        />
      </CardContent>
    </Card>
  );
}
