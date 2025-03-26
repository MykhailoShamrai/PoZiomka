import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthGatewayService {
  private apiUrl = `${environment.apiUrl}/Auth`;

  constructor(private http: HttpClient) {
  }

  // TODO: login

  register(email: string, password: string): Observable<any> {
    return this.http.post(this.apiUrl + '/register', {
      email,
      password,
    });
  }
}
