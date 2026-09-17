import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CriterionPercentageAllocation,
  CriterionDirectRanking
} from '../models/weighting.model';

@Injectable({
  providedIn: 'root'
})
export class WeightingService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7158/api/Decisions';

  setPercentageAllocation(
    decisionId: number,
    weights: CriterionPercentageAllocation[]
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${decisionId}/weight`,
      weights
    );
  }

  setDirectRanking(
    decisionId: number,
    rankings: CriterionDirectRanking[]
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${decisionId}/ranking`,
      rankings
    );
  }
}