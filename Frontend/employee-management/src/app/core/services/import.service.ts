// core/services/import.service.ts
// Service xử lý import nhân viên từ CSV

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ImportResponse } from '../models/import.model';

@Injectable({
  providedIn: 'root'
})
export class ImportService {
  private baseUrl = '/api/import';

  constructor(private http: HttpClient) {}

  importEmployees(file: File): Observable<ImportResponse> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<ImportResponse>(`${this.baseUrl}/employees`, formData);
  }
}