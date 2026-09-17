import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CriterionNumericalRule,
  CreateCriterionNumericalRuleRequest,
  UpdateCriterionNumericalRuleRequest
} from '../models/criterion-numerical-rule.model';

@Injectable({
  providedIn: 'root'
})
export class CriterionNumericalRuleService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7158/api/CriterionNumericalRule';

  getRules(): Observable<CriterionNumericalRule[]> {
    return this.http.get<CriterionNumericalRule[]>(
      this.apiUrl
    );
  }

  getRule(id: number): Observable<CriterionNumericalRule> {
    return this.http.get<CriterionNumericalRule>(
      `${this.apiUrl}/${id}`
    );
  }

  createRule(
    request: CreateCriterionNumericalRuleRequest
  ): Observable<CriterionNumericalRule> {
    return this.http.post<CriterionNumericalRule>(
      this.apiUrl,
      request
    );
  }

  updateRule(
    id: number,
    request: UpdateCriterionNumericalRuleRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deleteRule(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}