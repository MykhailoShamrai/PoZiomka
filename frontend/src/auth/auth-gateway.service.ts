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
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    // Check if token exists on service instantiation
    this.isAuthenticatedSubject.next(this.hasToken());
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

  private hasToken(): boolean {
    return !!this.getToken();
  }

  private handleAuthResponse(response: AuthResponse): void {
    if (response && response.token) {
      localStorage.setItem(this.tokenKey, response.token);
      localStorage.setItem(this.userEmailKey, response.email);
      this.isAuthenticatedSubject.next(true);
    }
  }
}