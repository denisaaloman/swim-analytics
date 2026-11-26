import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { LeaderboardEntry } from '../models/leaderboard-entry';
import { PagedResult } from '../models/paged-result';
import { PersonalBest, PoolLength } from '../models/personal-best';
import { ProgressionPoint } from '../models/progression-point';
import { CreateResultRequest, Pacing, Points, SwimResult } from '../models/result';

@Injectable({ providedIn: 'root' })
export class ResultsApiService {
  private readonly http = inject(HttpClient);

  getPage(swimmerId: number, page: number, pageSize: number): Observable<PagedResult<SwimResult>> {
    const params = new HttpParams().set('swimmerId', swimmerId).set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<SwimResult>>(`${API_URL}/results`, { params });
  }

  getById(id: number): Observable<SwimResult> {
    return this.http.get<SwimResult>(`${API_URL}/results/${id}`);
  }

  create(request: CreateResultRequest): Observable<SwimResult> {
    return this.http.post<SwimResult>(`${API_URL}/results`, request);
  }

  getPoints(id: number): Observable<Points> {
    return this.http.get<Points>(`${API_URL}/results/${id}/points`);
  }

  getPacing(id: number): Observable<Pacing> {
    return this.http.get<Pacing>(`${API_URL}/results/${id}/pacing`);
  }

  getPersonalBests(swimmerId: number): Observable<PersonalBest[]> {
    return this.http.get<PersonalBest[]>(`${API_URL}/results/personal-bests`, { params: { swimmerId } });
  }

  getProgression(swimmerId: number, eventId: number): Observable<ProgressionPoint[]> {
    return this.http.get<ProgressionPoint[]>(`${API_URL}/results/progression`, { params: { swimmerId, eventId } });
  }

  getLeaderboard(
    eventId: number,
    poolLength: PoolLength,
    page: number,
    pageSize: number,
  ): Observable<PagedResult<LeaderboardEntry>> {
    const params = new HttpParams()
      .set('eventId', eventId)
      .set('poolLength', poolLength)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PagedResult<LeaderboardEntry>>(`${API_URL}/results/leaderboard`, { params });
  }
}
