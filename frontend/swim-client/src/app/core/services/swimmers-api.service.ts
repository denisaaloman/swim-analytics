import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { PagedResult } from '../models/paged-result';
import { Swimmer } from '../models/swimmer';

@Injectable({ providedIn: 'root' })
export class SwimmersApiService {
  private readonly http = inject(HttpClient);

  getPage(page: number, pageSize: number, search?: string): Observable<PagedResult<Swimmer>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    return this.http.get<PagedResult<Swimmer>>(`${API_URL}/swimmers`, { params });
  }

  getById(id: number): Observable<Swimmer> {
    return this.http.get<Swimmer>(`${API_URL}/swimmers/${id}`);
  }
}
