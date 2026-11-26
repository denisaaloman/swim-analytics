import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { PagedResult } from '../../core/models/paged-result';
import { Swimmer } from '../../core/models/swimmer';
import { SwimmersApiService } from '../../core/services/swimmers-api.service';

@Component({
  selector: 'app-swimmers-list',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './swimmers-list.component.html',
})
export class SwimmersListComponent {
  private readonly api = inject(SwimmersApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly pageSize = 20;
  readonly search = new FormControl('', { nonNullable: true });

  readonly data = signal<PagedResult<Swimmer> | null>(null);
  readonly page = signal(1);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly totalPages = computed(() => {
    const d = this.data();
    return d ? Math.max(1, Math.ceil(d.totalCount / d.pageSize)) : 1;
  });

  constructor() {
    this.load();

    this.search.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });
  }

  goTo(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.page.set(page);
    this.load();
  }

  private load() {
    this.loading.set(true);
    this.error.set(null);

    this.api
      .getPage(this.page(), this.pageSize, this.search.value)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.data.set(result);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Nu am putut încărca înotătorii.');
          this.loading.set(false);
        },
      });
  }
}
