import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CriterionOption,
  CreateCriterionOptionsRequest,
  UpdateCriterionOptionsRequest
} from '../models/criterion-option.model';

@Injectable({
  providedIn: 'root'
})
export class CriterionOptionService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7158/api/CriterionOption';

  getOptions(): Observable<CriterionOption[]> {
    return this.http.get<CriterionOption[]>(
      this.apiUrl
    );
  }

  getOption(id: number): Observable<CriterionOption> {
    return this.http.get<CriterionOption>(
      `${this.apiUrl}/${id}`
    );
  }

  createOptions(
    request: CreateCriterionOptionsRequest
  ): Observable<CriterionOption[]> {
    return this.http.post<CriterionOption[]>(
      this.apiUrl,
      request
    );
  }

  updateOptions(
    criterionId: number,
    request: UpdateCriterionOptionsRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/criterion/${criterionId}`,
      request
    );
  }

  deleteOption(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}