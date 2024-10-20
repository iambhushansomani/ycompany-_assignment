import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { map, Observable, of, switchMap } from 'rxjs';
import { AuthService } from './auth.service';



@Injectable({
  providedIn: 'root'
})

export class ClaimService {
  private apiUrl = 'http://api.ycompany.local/claims/api';

  constructor(private http: HttpClient, private authService: AuthService) { }

  initiateClaim(claimData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/Claims?userId=${this.authService.currentUserId}`, claimData);
  }

  selectWorkshop(claimId: string, workshopId: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/workshop`, { workshopId });
  }

  submitClaim(claimId: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/submit`, {});
  }

  getClaimDetails(claimId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/Claims/${claimId}`);
  }

  approveClaim(claimId: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/approve`, {});
  }

  rejectClaim(claimId: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/reject`, {});
  }

  getWorkshops(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Workshop`);
  }

  assignWorkshopToClaim(claimId: number, workshopId: string, options?: { headers?: HttpHeaders }): Observable<any> {
    const url = `${this.apiUrl}/Claims/${claimId}/workshop`;
    return this.http.put(url, `"${workshopId}"`, options);
  }

  getClaims(): Observable<any[]> {
    if (this.authService.isSurveyor()) {
      return this.getSurveyorClaims();
    } else if (this.authService.isAdjuster()) {
      return this.getAdjusterClaims();
    } else if (this.authService.isWorkshop()) {
      return this.getWorkshopClaims();
    } else if (this.authService.isManager()) {
      return this.getAllClaims();
    } else {
      const userId = this.authService.currentUserId;
      return this.http.get<any[]>(`${this.apiUrl}/Claims/user/${userId}`);
    }
  }

  getAllClaims(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Claims/all`);
  }

  getSurveyorClaims(): Observable<any[]> {
    const surveyorId = this.authService.currentUserId;
    if (!surveyorId) {
      throw new Error('Surveyor ID is null');
    }
    return this.getAllClaims().pipe(
      map((claims: any[]) => claims.filter(claim => claim.SurveyorId === parseInt(surveyorId, 10)))
    );
  }

  getWorkshopClaims(): Observable<any[]> {
    const workshopId = this.authService.getWorkshopId();
    if (!workshopId) {
      throw new Error('Workshop ID is null');
    }
    return this.getAllClaims().pipe(
      map((claims: any[]) => claims.filter(claim => claim.SelectedWorkshopId === parseInt(workshopId, 10)))
    );
  }

  updateClaim(claimId: number, claimData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/Claims/${claimId}`, claimData);
  }

  uploadInvoice(claimId: number, amount: number, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    formData.append('amount', amount.toString());

    return this.http.post(`${this.apiUrl}/Claims/${claimId}/upload-invoice`, formData);
  }

  getInvoice(claimId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/Claims/${claimId}/invoice`, { responseType: 'blob' });
  }

  payInvoice(claimId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/Claims/${claimId}/process-payment`, {});
  }

  getAdjusterClaims(): Observable<any[]> {
    const adjusterId = this.authService.currentUserId;
    if (!adjusterId) {
      throw new Error('Adjuster ID is null');
    }
    return this.getAllClaims().pipe(
      map((claims: any[]) => claims.filter(claim => claim.AdjustorId === parseInt(adjusterId, 10)))
    );
  }

  createAssessment(assessmentData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Assessments`, assessmentData).pipe(
      switchMap(response => {
        if (assessmentData['status'] === 'Surveyor Approved' || assessmentData['status'] === 'Adjustor Approved') {
          return this.assignAdjuster(assessmentData['claimId']);
        }
        return of(response);
      })
    );
  }

  getAssessmentDetails(claimId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/Assessments/${claimId}`);
  }
  
  adjustClaim(claimId: number, adjustment: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/adjudicate`, adjustment);
  }

  private assignAdjuster(claimId: number): Observable<any> {
    return this.authService.getAvailableAdjusters().pipe(
      switchMap(adjusters => {
        if (adjusters && adjusters.length > 0) {
          const adjusterId = adjusters[0].Id; 
          return this.assignAdjusterToClaim(claimId, parseInt(adjusterId, 10));
        } else {
          throw new Error('No available adjusters');
        }
      })
    );
  }

  

  assignSurveyorToClaim(claimId: number, surveyorId: number): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/assign-surveyor`, surveyorId, { headers });
  }

  assignAdjusterToClaim(claimId: number, adjusterId: number): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/assign-adjustor`, adjusterId, { headers });
  }

  

  updateClaimStatus(claimId: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/Claims/${claimId}/status`, JSON.stringify(status), {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    });
  }
}