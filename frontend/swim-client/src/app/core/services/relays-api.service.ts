import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { PoolLength } from '../models/personal-best';
import { Relay } from '../models/relay';
import { Gender } from '../models/swimmer';

@Injectable({ providedIn: 'root' })
export class RelaysApiService {
  private readonly http = inject(HttpClient);

  getMedley(gender: Gender, poolLength: PoolLength, clubId: number | null): Observable<Relay> {
    let params = new HttpParams().set('gender', gender).set('poolLength', poolLength);
    if (clubId !== null) params = params.set('clubId', clubId);
    return this.http.get<Relay>(`${API_URL}/relays/medley`, { params });
  }
}
