import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Criterion,
  CreateCriterionRequest
} from '../models/criterion.model';

export interface UpdateCriterionRequest {
  decisionId: number;
  name: string;
  criterionType: Criterion['criterionType'];
  unit: string | null;
  weight: number | null;
}

@Injectable({
  providedIn: 'root'
})
export class CriterionService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7158/api/Criterion';

  getCriteria(): Observable<Criterion[]> {
    return this.http.get<Criterion[]>(this.apiUrl);
  }

  createCriterion(
    request: CreateCriterionRequest
  ): Observable<Criterion> {
    return this.http.post<Criterion>(
      this.apiUrl,
      request
    );
  }

  updateCriterion(
  id: number,
  request: UpdateCriterionRequest
): Observable<void> {
  return this.http.put<void>(
    `${this.apiUrl}/${id}`,
    request
  );
}

  deleteCriterion(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}