import { Component, DestroyRef, ElementRef, effect, inject, input, viewChild } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { Segment } from '../core/models/result';
import { formatTime } from '../core/utils/format';

Chart.register(...registerables);

@Component({
  selector: 'app-pacing-chart',
  template: `<canvas #canvas></canvas>`,
  styles: `:host { display: block; max-width: 700px; }`,
})
export class PacingChartComponent {
  readonly segments = input.required<Segment[]>();

  private readonly canvas = viewChild<ElementRef<HTMLCanvasElement>>('canvas');
  private chart?: Chart;

  constructor() {
    inject(DestroyRef).onDestroy(() => this.chart?.destroy());

    effect(() => {
      const el = this.canvas()?.nativeElement;
      const segs = this.segments();
      if (!el) return;

      this.chart?.destroy();
      this.chart = new Chart(el, {
        type: 'bar',
        data: {
          labels: segs.map((s) => `${s.fromMeters}-${s.toMeters}m`),
          datasets: [{ data: segs.map((s) => s.per50Ms / 1000) }],
        },
        options: {
          plugins: {
            legend: { display: false },
            tooltip: { callbacks: { label: (item) => `${formatTime((item.parsed.y ?? 0) * 1000)} / 50m` } },
          },
          scales: {
            y: {
              beginAtZero: false,
              title: { display: true, text: 'Timp la 50m' },
              ticks: { callback: (v) => formatTime(Number(v) * 1000) },
            },
          },
        },
      });
    });
  }
}
