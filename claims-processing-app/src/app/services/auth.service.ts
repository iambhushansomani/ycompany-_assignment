import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import * as jwtDecode from 'jwt-decode';
interface DecodedToken {
  WorkshopId?: any;
  nameid: string;
  unique_name: string;
  role: string;
  exp: number;
}

interface User {
  id: string;
  username: string;
  role: string;
  token: string;
  workshopId?: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://api.ycompany.local/auth/api';
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;
  public isLoggedIn$: Observable<boolean>;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromLocalStorage());
    this.currentUser = this.currentUserSubject.asObservable();
    this.isLoggedIn$ = this.currentUser.pipe(map(user => !!user));
  }

  getSurveyors(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/Auth/by-role/Surveyor`);
  }

  private getUserFromLocalStorage(): User | null {
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      const { token } = JSON.parse(storedUser);
      const decodedToken = this.decodeToken(token);
      if (decodedToken && this.isTokenValid(decodedToken)) {
        return {
          id: decodedToken.nameid,
          username: decodedToken.unique_name,
          role: decodedToken.role,
          token: token,
          workshopId: decodedToken.WorkshopId ? parseInt(decodedToken.WorkshopId) : null
        };
      }
    }
    return null;
  }

  private decodeToken(token: string): DecodedToken | null {
    try {
      return jwtDecode.jwtDecode<DecodedToken>(token);
    } catch (error) {
      console.error('Error decoding token', error);
      return null;
    }
  }

  private isTokenValid(decodedToken: DecodedToken): boolean {
    return decodedToken.exp * 1000 > Date.now();
  }
  

  public get name(): string | null {
    return this.decodeToken(this.currentUserValue?.token || '')?.unique_name || null;
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get currentUserId(): string | null {
    return this.currentUserValue?.id || null;
  }

  public get userRole(): string | null {
    return this.currentUserValue?.role || null;
  }

  public hasRole(role: string): boolean {
    const user = this.currentUserValue;
    return user ? user.role === role : false;
  }

  public isCustomer(): boolean {
    return this.hasRole('Customer');
  }

  public isManager(): boolean {
    return this.hasRole('ClaimsManager');
  }

  public isAdjuster(): boolean {
    return this.hasRole('Adjustor');
  }

  public isWorkshop(): boolean {
    return this.hasRole('Workshop');
  }

  public isSurveyor(): boolean {
    return this.hasRole('Surveyor');
  }

  public isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  public canAccessClaimList(): boolean {
    return this.isCustomer() || this.isAdmin();
  }

  public canAccessReports(): boolean {
    return this.isAdmin();
  }

  login(username: string, password: string): Observable<User> {
    return this.http.post<{ Token: string }>(`${this.apiUrl}/Auth/login`, { username, password })
      .pipe(
        map(response => {
          const decodedToken = this.decodeToken(response.Token);
          if (decodedToken) {
            const user: User = {
              id: decodedToken.nameid,
              username: decodedToken.unique_name,
              role: decodedToken.role,
              token: response.Token,
              workshopId: decodedToken.WorkshopId ? parseInt(decodedToken.WorkshopId) : null
            };
            localStorage.setItem('currentUser', JSON.stringify({ token: response.Token }));
            this.currentUserSubject.next(user);
            return user;
          } else {
            throw new Error('Invalid token');
          }
        })
      );
  }

  public getAvailableAdjusters(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Auth/by-role/Adjustor`);
  }



  getUserId(): string | null {
    const token = localStorage.getItem('currentUser');
    if (token) {
      const decodedToken = this.decodeToken(token);
      console.log(decodedToken)

      if (decodedToken) {
        return decodedToken.nameid; // Assuming 'nameid' is the claim for userId in your JWT
      }
    }
    return null;
  }

  getWorkshopId(): string | null {
    const token = localStorage.getItem('currentUser');
    if (token) {
      const decodedToken = this.decodeToken(token);
      console.log(decodedToken)

      if (decodedToken) {
        return decodedToken.WorkshopId; // Assuming 'nameid' is the claim for userId in your JWT
      }
    }
    return null;
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}