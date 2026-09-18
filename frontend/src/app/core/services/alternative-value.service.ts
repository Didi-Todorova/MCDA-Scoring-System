import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  AlternativeValue,
  CreateAlternativeValueRequest,
  UpdateAlternativeValueRequest,
  PatchAlternativeValueRequest
} from '../models/alternative-value.model';

@Injectable({
  providedIn: 'root'
})
export class AlternativeValueService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7158/api/AlternativeValue';

  getAlternativeValues(): Observable<AlternativeValue[]> {
    return this.http.get<AlternativeValue[]>(
      this.apiUrl
    );
  }

  getAlternativeValue(
    id: number
  ): Observable<AlternativeValue> {
    return this.http.get<AlternativeValue>(
      `${this.apiUrl}/${id}`
    );
  }

  createAlternativeValue(
    request: CreateAlternativeValueRequest
  ): Observable<AlternativeValue> {
    return this.http.post<AlternativeValue>(
      this.apiUrl,
      request
    );
  }

  updateAlternativeValue(
    id: number,
    request: UpdateAlternativeValueRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  patchAlternativeValue(
    id: number,
    request: PatchAlternativeValueRequest
  ): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deleteAlternativeValue(
    id: number
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}