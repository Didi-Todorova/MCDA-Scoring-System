import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  IntervalRange,
  CreateIntervalRangesRequest,
  UpdateIntervalRangesRequest
} from '../models/interval-range.model';

@Injectable({
  providedIn: 'root'
})
export class IntervalRangeService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7158/api/IntervalRange';

  getRanges(): Observable<IntervalRange[]> {
    return this.http.get<IntervalRange[]>(
      this.apiUrl
    );
  }

  getRange(id: number): Observable<IntervalRange> {
    return this.http.get<IntervalRange>(
      `${this.apiUrl}/${id}`
    );
  }

  getRangesByRule(
    ruleId: number
  ): Observable<IntervalRange[]> {
    return this.http.get<IntervalRange[]>(
      `${this.apiUrl}/rule/${ruleId}`
    );
  }

  createRanges(
    request: CreateIntervalRangesRequest
  ): Observable<IntervalRange[]> {
    return this.http.post<IntervalRange[]>(
      this.apiUrl,
      request
    );
  }

  updateRanges(
    ruleId: number,
    request: UpdateIntervalRangesRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/rule/${ruleId}`,
      request
    );
  }

  deleteRange(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}