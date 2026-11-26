import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormControl, FormGroup, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs';
import { SwimEvent } from '../../core/models/event';
import { Meet } from '../../core/models/meet';
import { Swimmer } from '../../core/models/swimmer';
import { EventsApiService } from '../../core/services/events-api.service';
import { MeetsApiService } from '../../core/services/meets-api.service';
import { ResultsApiService } from '../../core/services/results-api.service';
import { SwimmersApiService } from '../../core/services/swimmers-api.service';
import { eventLabel, parseTime, poolLabel } from '../../core/utils/format';

type SplitGroup = FormGroup<{ distance: FormControl<number>; time: FormControl<string> }>;

@Component({
  selector: 'app-result-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './result-form.component.html',
})
export class ResultFormComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly swimmersApi = inject(SwimmersApiService);
  private readonly eventsApi = inject(EventsApiService);
  private readonly meetsApi = inject(MeetsApiService);
  private readonly resultsApi = inject(ResultsApiService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly eventLabel = (e: SwimEvent) => `${eventLabel(e.distanceMeters, e.stroke)} ${e.gender === 'Male' ? 'M' : 'F'}`;
  readonly poolLabel = poolLabel;

  readonly events = signal<SwimEvent[]>([]);
  readonly meets = signal<Meet[]>([]);

  readonly swimmerSearch = new FormControl('', { nonNullable: true });
  readonly swimmerOptions = signal<Swimmer[]>([]);
  readonly swimmer = signal<Swimmer | null>(null);

  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.group({
    eventId: this.fb.control<number | null>(null, Validators.required),
    meetId: this.fb.control<number | null>(null, Validators.required),
    time: this.fb.control('', Validators.required),
    splits: this.fb.array<SplitGroup>([]),
  });

  get splits(): FormArray<SplitGroup> {
    return this.form.controls.splits;
  }

  constructor() {
    this.eventsApi.getAll().pipe(takeUntilDestroyed(this.destroyRef)).subscribe((e) => this.events.set(e));
    this.meetsApi.getAll().pipe(takeUntilDestroyed(this.destroyRef)).subscribe((m) => this.meets.set(m.items));

    // dacă vii de pe pagina unui înotător, e deja ales
    const swimmerId = inject(ActivatedRoute).snapshot.queryParamMap.get('swimmerId');
    if (swimmerId) {
      this.swimmersApi
        .getById(Number(swimmerId))
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({ next: (s) => this.swimmer.set(s), error: () => {} });
    }

    // căutare înotător, cu debounce
    this.swimmerSearch.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        filter((term) => term.trim().length >= 2),
        switchMap((term) => this.swimmersApi.getPage(1, 8, term.trim())),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((page) => this.swimmerOptions.set(page.items));

    // când alegi proba, generăm câte un câmp de split la fiecare 50m
    this.form.controls.eventId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((id) => {
      const ev = this.events().find((e) => e.id === id);
      this.splits.clear();
      if (!ev) return;
      for (let d = 50; d < ev.distanceMeters; d += 50) this.splits.push(this.newSplit(d));
    });
  }

  pickSwimmer(s: Swimmer) {
    this.swimmer.set(s);
    this.swimmerOptions.set([]);
    this.swimmerSearch.setValue('', { emitEvent: false });
  }

  clearSwimmer() {
    this.swimmer.set(null);
  }

  submit() {
    this.error.set(null);

    const swimmer = this.swimmer();
    if (!swimmer) {
      this.error.set('Alege un înotător.');
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    const timeMs = parseTime(value.time);
    if (timeMs === null) {
      this.error.set('Timp invalid. Exemple: 55.23 sau 1:55.40');
      return;
    }

    const splits: { distanceMeters: number; cumulativeTimeMs: number }[] = [];
    for (const s of value.splits) {
      if (!s.time.trim()) continue; // split necompletat = ignorat
      const ms = parseTime(s.time);
      if (ms === null) {
        this.error.set(`Split invalid la ${s.distance}m. Exemple: 26.10 sau 1:01.20`);
        return;
      }
      splits.push({ distanceMeters: s.distance, cumulativeTimeMs: ms });
    }

    this.saving.set(true);
    this.resultsApi
      .create({ swimmerId: swimmer.id, eventId: value.eventId!, meetId: value.meetId!, timeMs, splits })
      .subscribe({
        next: (result) => this.router.navigate(['/results', result.id]),
        error: (err: HttpErrorResponse) => {
          this.error.set(this.messageFrom(err));
          this.saving.set(false);
        },
      });
  }

  private newSplit(distance: number): SplitGroup {
    return this.fb.group({ distance: this.fb.control(distance), time: this.fb.control('') });
  }

  private messageFrom(err: HttpErrorResponse): string {
    const body = err.error;
    if (typeof body === 'string' && body) return body; // BadRequest("...") din service
    if (body?.errors) return Object.values<string[]>(body.errors).flat().join(' '); // validarea automată
    return 'Nu am putut salva rezultatul.';
  }
}
