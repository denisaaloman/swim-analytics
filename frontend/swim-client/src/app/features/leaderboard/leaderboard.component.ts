import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { SwimEvent } from '../../core/models/event';
import { LeaderboardEntry } from '../../core/models/leaderboard-entry';
import { PagedResult } from '../../core/models/paged-result';
import { PoolLength } from '../../core/models/personal-best';
import { EventsApiService } from '../../core/services/events-api.service';
import { ResultsApiService } from '../../core/services/results-api.service';
import { eventLabel, formatTime } from '../../core/utils/format';

@Component({
  selector: 'app-leaderboard',
  imports: [RouterLink],
  templateUrl: './leaderboard.component.html',
})
export class LeaderboardComponent {
  private readonly eventsApi = inject(EventsApiService);
  private readonly resultsApi = inject(ResultsApiService);
  private readonly destroyRef = inject(DestroyRef);
  private sub?: Subscription;

  readonly pageSize = 20;
  readonly formatTime = formatTime;
  readonly label = (e: SwimEvent) => `${eventLabel(e.distanceMeters, e.stroke)} ${e.gender === 'Male' ? 'M' : 'F'}`;

  readonly events = signal<SwimEvent[]>([]);
  readonly eventId = signal<number | null>(null);
  readonly pool = signal<PoolLength>('Long');
  readonly page = signal(1);

  readonly data = signal<PagedResult<LeaderboardEntry> | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly totalPages = computed(() => {
    const d = this.data();
    return d ? Math.max(1, Math.ceil(d.totalCount / d.pageSize)) : 1;
  });

  constructor() {
    this.eventsApi
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (events) => {
          this.events.set(events);
          if (events.length > 0) {
            this.eventId.set(events[0].id);
            this.load();
          }
        },
        error: () => this.error.set('Nu am putut încărca probele.'),
      });

    this.destroyRef.onDestroy(() => this.sub?.unsubscribe());
  }

  onEventChange(value: string) {
    this.eventId.set(Number(value));
    this.page.set(1);
    this.load();
  }

  onPoolChange(value: string) {
    this.pool.set(value as PoolLength);
    this.page.set(1);
    this.load();
  }

  goTo(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.page.set(page);
    this.load();
  }

  private load() {
    const eventId = this.eventId();
    if (eventId === null) return;

    this.sub?.unsubscribe(); // anulează request-ul anterior dacă user-ul schimbă filtrele repede
    this.loading.set(true);
    this.error.set(null);

    this.sub = this.resultsApi.getLeaderboard(eventId, this.pool(), this.page(), this.pageSize).subscribe({
      next: (result) => {
        this.data.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Nu am putut încărca clasamentul.');
        this.loading.set(false);
      },
    });
  }
}
