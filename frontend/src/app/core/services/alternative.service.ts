import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Alternative,
  CreateAlternativeRequest,
  UpdateAlternativeRequest,
  PatchAlternativeRequest
} from '../models/alternative.model';

@Injectable({
  providedIn: 'root'
})
export class AlternativeService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7158/api/Alternatives';

  getAlternatives(): Observable<Alternative[]> {
    return this.http.get<Alternative[]>(this.apiUrl);
  }

  getAlternative(id: number): Observable<Alternative> {
    return this.http.get<Alternative>(
      `${this.apiUrl}/${id}`
    );
  }

  createAlternative(
    request: CreateAlternativeRequest
  ): Observable<Alternative> {
    return this.http.post<Alternative>(
      this.apiUrl,
      request
    );
  }

  updateAlternative(
    id: number,
    request: UpdateAlternativeRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  patchAlternative(
    id: number,
    request: PatchAlternativeRequest
  ): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deleteAlternative(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}