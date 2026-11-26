import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Subject, catchError, forkJoin, map, of, switchMap, tap } from 'rxjs';
import { PersonalBest } from '../../core/models/personal-best';
import { ProgressionPoint } from '../../core/models/progression-point';
import { SwimResult } from '../../core/models/result';
import { Swimmer } from '../../core/models/swimmer';
import { ResultsApiService } from '../../core/services/results-api.service';
import { SwimmersApiService } from '../../core/services/swimmers-api.service';
import { eventLabel, formatTime, poolLabel } from '../../core/utils/format';
import { ProgressionChartComponent } from '../../shared/progression-chart.component';

@Component({
  selector: 'app-swimmer-detail',
  imports: [RouterLink, ProgressionChartComponent],
  templateUrl: './swimmer-detail.component.html',
})
export class SwimmerDetailComponent {
  private readonly swimmersApi = inject(SwimmersApiService);
  private readonly resultsApi = inject(ResultsApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly id = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));

  readonly formatTime = formatTime;
  readonly poolLabel = poolLabel;
  readonly eventLabel = eventLabel;
  readonly label = (pb: PersonalBest) => eventLabel(pb.distanceMeters, pb.stroke);

  readonly swimmer = signal<Swimmer | null>(null);
  readonly bests = signal<PersonalBest[]>([]);
  readonly results = signal<SwimResult[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly selected = signal<PersonalBest | null>(null);
  readonly progression = signal<ProgressionPoint[]>([]);

  private readonly selection$ = new Subject<PersonalBest>();

  constructor() {
    forkJoin({
      swimmer: this.swimmersApi.getById(this.id),
      bests: this.resultsApi.getPersonalBests(this.id),
      results: this.resultsApi.getPage(this.id, 1, 20),
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ swimmer, bests, results }) => {
          this.swimmer.set(swimmer);
          this.bests.set(bests);
          this.results.set(results.items);
          this.loading.set(false);
          if (bests.length > 0) this.select(bests[0]);
        },
        error: () => {
          this.error.set('Nu am găsit înotătorul.');
          this.loading.set(false);
        },
      });

    // switchMap: dacă dai click repede pe alt rând, request-ul vechi e anulat
    this.selection$
      .pipe(
        tap((pb) => {
          this.selected.set(pb);
          this.progression.set([]);
        }),
        switchMap((pb) =>
          this.resultsApi.getProgression(this.id, pb.eventId).pipe(
            // endpointul întoarce ambele tipuri de bazin; păstrăm doar pe cel ales
            map((points) => points.filter((p) => p.poolLength === pb.poolLength)),
            catchError(() => of([] as ProgressionPoint[])),
          ),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((points) => this.progression.set(points));
  }

  select(pb: PersonalBest) {
    this.selection$.next(pb);
  }
}
