import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import * as bcrypt from 'bcryptjs';
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
    const saltRounds = 10;
    const hashedPassword = bcrypt.hashSync(password, saltRounds);

    return this.http.post(this.apiUrl + '/register', {
      email,
      hashedPassword,
    });
  }
}
