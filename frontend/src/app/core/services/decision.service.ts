import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Decision, WeightingMethod } from '../models/decision.model';

import {
  DecisionScore
} from '../models/decision-score.model';

export interface CreateDecisionRequest {
  name: string;
  weightingMethod: WeightingMethod;
}

export interface UpdateDecisionRequest {
  name: string;
  weightingMethod: WeightingMethod;
}

@Injectable({
  providedIn: 'root'
})
export class DecisionService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'https://localhost:7158/api/Decisions';

  getDecisions(): Observable<Decision[]> {
    return this.http.get<Decision[]>(this.apiUrl);
  }

  getDecision(id: number): Observable<Decision> {
    return this.http.get<Decision>(`${this.apiUrl}/${id}`);
  }

  createDecision(
    request: CreateDecisionRequest
  ): Observable<Decision> {
    return this.http.post<Decision>(this.apiUrl, request);
  }

  updateDecision(
    id: number,
    request: UpdateDecisionRequest
  ): Observable<Decision> {
    return this.http.put<Decision>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deleteDecision(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getDecisionScore(
    decisionId: number
  ): Observable<DecisionScore[]> {
    return this.http.get<DecisionScore[]>(
      `${this.apiUrl}/${decisionId}/score`
    );
  }
}