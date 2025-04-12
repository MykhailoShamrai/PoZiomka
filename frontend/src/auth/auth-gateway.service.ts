import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../environment/environment';
import { Router } from '@angular/router';

interface AuthResponse {
  token: string;
  email: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthGatewayService {
  private apiUrl = `${environment.apiUrl}Auth`;
  private tokenKey = 'auth_token';
  private userEmailKey = 'user_email';
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.isAuthenticated());
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    // Check if token exists on service instantiation
    this.isAuthenticatedSubject.next(this.isAuthenticated());
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.apiUrl + '/login', {
      email,
      password,
    }).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  register(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.apiUrl + '/register', {
      email,
      password,
    }).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  logout(): void {
    this.http.post(this.apiUrl + '/logout', {}).subscribe();
    
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userEmailKey);
    this.isAuthenticatedSubject.next(false);
    
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getUserEmail(): string | null {
    return localStorage.getItem(this.userEmailKey);
  }

  private isAuthenticated(): boolean {
    return !!this.getUserEmail();
  }

  private handleAuthResponse(response: AuthResponse): void {
    if (response && response.email) {
      localStorage.setItem(this.userEmailKey, response.email);
      this.isAuthenticatedSubject.next(true);
    }
  }
}