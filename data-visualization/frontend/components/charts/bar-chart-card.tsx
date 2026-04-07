"use client";

import ReactECharts from "echarts-for-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { GroupedMetricDto } from "@/types/experiments";

export function BarChartCard({
  title,
  data,
  metric,
  color = "#ea580c"
}: {
  title: string;
  data: GroupedMetricDto[];
  metric: keyof GroupedMetricDto;
  color?: string;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
      </CardHeader>
      <CardContent className="px-3 pb-3 sm:px-6 sm:pb-6">
        <ReactECharts
          style={{ height: 240 }}
          option={{
            tooltip: { trigger: "axis" },
            grid: { left: 28, right: 12, top: 24, bottom: 48, containLabel: true },
            xAxis: {
              type: "category",
              data: data.map((item) => item.groupKey),
              axisLabel: { interval: 0, rotate: data.length > 3 ? 20 : 0, fontSize: 11 }
            },
            yAxis: { type: "value", axisLabel: { fontSize: 11 } },
            series: [
              {
                type: "bar",
                barWidth: 42,
                itemStyle: { color, borderRadius: [12, 12, 0, 0] },
                data: data.map((item) => item[metric] as number)
              }
            ]
          }}
        />
      </CardContent>
    </Card>
  );
}
