import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private apiUrl = 'http://api.ycompany.local/documents/api';

  constructor(private http: HttpClient) { }

  uploadDocuments(claimId: string, formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/documents/${claimId}`, formData);
  }

  getDocuments(claimId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/documents/${claimId}`);
  }
}