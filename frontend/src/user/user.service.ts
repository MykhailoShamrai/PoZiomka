import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError, of, delay } from 'rxjs';
import { environment } from '../environment/environment';

export interface UserProfile {
  email: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
}

export interface UserForm {
  email: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = `${environment.apiUrl}User`;

  constructor(private http: HttpClient) {}

  getUserProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/profile`).pipe(
      catchError(this.handleError)
    );
  }

  updateUserProfile(userData: UserProfile): Observable<any> {
    return this.http.put(`${this.apiUrl}/profile`, userData).pipe(
      catchError(this.handleError)
    );
  }

  getUserForms(): Observable<UserForm[]> {
    return this.http.get<UserForm[]>(`${this.apiUrl}/forms`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}