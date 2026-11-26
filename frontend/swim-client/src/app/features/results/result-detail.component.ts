import { DecimalPipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, forkJoin, of } from 'rxjs';
import { Pacing, Points, SwimResult } from '../../core/models/result';
import { ResultsApiService } from '../../core/services/results-api.service';
import { eventLabel, formatTime, poolLabel, profileLabel } from '../../core/utils/format';
import { PacingChartComponent } from '../../shared/pacing-chart.component';

@Component({
  selector: 'app-result-detail',
  imports: [RouterLink, DecimalPipe, PacingChartComponent],
  templateUrl: './result-detail.component.html',
})
export class ResultDetailComponent {
  private readonly api = inject(ResultsApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly id = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));

  readonly formatTime = formatTime;
  readonly poolLabel = poolLabel;
  readonly eventLabel = eventLabel;
  readonly profileLabel = profileLabel;

  readonly result = signal<SwimResult | null>(null);
  readonly points = signal<Points | null>(null);
  readonly pacing = signal<Pacing | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  // timpul cumulat la finalul fiecărui segment
  readonly rows = computed(() => {
    let cumulative = 0;
    return (this.pacing()?.segments ?? []).map((s) => {
      cumulative += s.timeMs;
      return { ...s, cumulativeMs: cumulative };
    });
  });

  constructor() {
    forkJoin({
      result: this.api.getById(this.id),
      // 400 = nu există timp de bază / nu are splits; nu e eroare fatală
      points: this.api.getPoints(this.id).pipe(catchError(() => of(null))),
      pacing: this.api.getPacing(this.id).pipe(catchError(() => of(null))),
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ result, points, pacing }) => {
          this.result.set(result);
          this.points.set(points);
          this.pacing.set(pacing);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Nu am găsit rezultatul.');
          this.loading.set(false);
        },
      });
  }
}
