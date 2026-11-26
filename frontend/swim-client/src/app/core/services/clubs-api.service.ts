import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { Club } from '../models/club';
import { PagedResult } from '../models/paged-result';

@Injectable({ providedIn: 'root' })
export class ClubsApiService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<PagedResult<Club>> {
    const params = new HttpParams().set('page', 1).set('pageSize', 100);
    return this.http.get<PagedResult<Club>>(`${API_URL}/clubs`, { params });
  }
}
