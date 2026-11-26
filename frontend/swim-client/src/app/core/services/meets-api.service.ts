import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { Meet } from '../models/meet';
import { PagedResult } from '../models/paged-result';

@Injectable({ providedIn: 'root' })
export class MeetsApiService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<PagedResult<Meet>> {
    const params = new HttpParams().set('page', 1).set('pageSize', 100);
    return this.http.get<PagedResult<Meet>>(`${API_URL}/meets`, { params });
  }
}
