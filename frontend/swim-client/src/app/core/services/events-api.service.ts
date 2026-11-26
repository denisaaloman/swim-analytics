import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { SwimEvent } from '../models/event';

@Injectable({ providedIn: 'root' })
export class EventsApiService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<SwimEvent[]> {
    return this.http.get<SwimEvent[]>(`${API_URL}/events`);
  }
}
