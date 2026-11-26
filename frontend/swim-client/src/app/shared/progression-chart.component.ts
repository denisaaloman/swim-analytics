import { Component, DestroyRef, ElementRef, effect, inject, input, viewChild } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { ProgressionPoint } from '../core/models/progression-point';
import { formatTime } from '../core/utils/format';

Chart.register(...registerables);

@Component({
  selector: 'app-progression-chart',
  template: `<canvas #canvas></canvas>`,
  styles: `:host { display: block; max-width: 700px; }`,
})
export class ProgressionChartComponent {
  readonly points = input.required<ProgressionPoint[]>();

  private readonly canvas = viewChild<ElementRef<HTMLCanvasElement>>('canvas');
  private chart?: Chart;

  constructor() {
    inject(DestroyRef).onDestroy(() => this.chart?.destroy());

    effect(() => {
      const el = this.canvas()?.nativeElement;
      const pts = this.points();
      if (!el) return;

      this.chart?.destroy();
      this.chart = new Chart(el, {
        type: 'line',
        data: {
          labels: pts.map((p) => p.date),
          datasets: [{ data: pts.map((p) => p.timeMs / 1000), tension: 0.2, pointRadius: 4 }],
        },
        options: {
          plugins: {
            legend: { display: false },
            tooltip: {
              callbacks: {
                title: (items) => pts[items[0].dataIndex].meetName,
                label: (item) => formatTime((item.parsed.y ?? 0) * 1000),
              },
            },
          },
          scales: {
            // reverse: un timp mai mic (mai bun) apare mai sus, deci progresul urcă pe grafic
            y: { reverse: true, ticks: { callback: (v) => formatTime(Number(v) * 1000) } },
          },
        },
      });
    });
  }
}
