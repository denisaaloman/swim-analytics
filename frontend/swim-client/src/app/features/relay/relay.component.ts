import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { Club } from '../../core/models/club';
import { PoolLength } from '../../core/models/personal-best';
import { Relay } from '../../core/models/relay';
import { Gender } from '../../core/models/swimmer';
import { ClubsApiService } from '../../core/services/clubs-api.service';
import { RelaysApiService } from '../../core/services/relays-api.service';
import { formatTime, strokeLabel } from '../../core/utils/format';

@Component({
  selector: 'app-relay',
  imports: [RouterLink],
  templateUrl: './relay.component.html',
})
export class RelayComponent {
  private readonly relaysApi = inject(RelaysApiService);
  private readonly clubsApi = inject(ClubsApiService);
  private readonly destroyRef = inject(DestroyRef);
  private sub?: Subscription;

  readonly formatTime = formatTime;
  readonly strokeLabel = strokeLabel;

  readonly clubs = signal<Club[]>([]);
  readonly gender = signal<Gender>('Male');
  readonly pool = signal<PoolLength>('Long');
  readonly clubId = signal<number | null>(null);

  readonly relay = signal<Relay | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  constructor() {
    this.clubsApi
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((result) => this.clubs.set(result.items));

    this.destroyRef.onDestroy(() => this.sub?.unsubscribe());
    this.load();
  }

  onGenderChange(value: string) {
    this.gender.set(value as Gender);
    this.load();
  }

  onPoolChange(value: string) {
    this.pool.set(value as PoolLength);
    this.load();
  }

  onClubChange(value: string) {
    this.clubId.set(value === '' ? null : Number(value));
    this.load();
  }

  private load() {
    this.sub?.unsubscribe();
    this.loading.set(true);
    this.error.set(null);
    this.relay.set(null);

    this.sub = this.relaysApi.getMedley(this.gender(), this.pool(), this.clubId()).subscribe({
      next: (relay) => {
        this.relay.set(relay);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.error.set(
          err.status === 404
            ? 'Nu sunt destui înotători cu rezultate la 100m pentru a forma o ștafetă.'
            : 'Nu am putut calcula ștafeta.',
        );
        this.loading.set(false);
      },
    });
  }
}
