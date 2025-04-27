import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

export interface FormCreateDto {
  formName: string;
  questions?: AddQuestionDto[];
}

export interface AddQuestionDto {
  formName: string;
  name: string;
  answers: string[];
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}admin`;

  constructor(private http: HttpClient) {}

  addNewForm(formDto: FormCreateDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/add_new_form`, formDto, { withCredentials: true });
  }

  addQuestion(preferenceDto: AddQuestionDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/add_question`, preferenceDto, { withCredentials: true });
  }

  deleteForm(nameOfForm: string): Observable<any> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.delete(`${this.apiUrl}/delete_form`, {
      headers: headers,
      body: JSON.stringify(nameOfForm),
      withCredentials: true
    });
  }

  deleteQuestion(formName: string, questionName: string): Observable<any> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    const body = { formName, questionName };
    return this.http.delete(`${this.apiUrl}/delete_question`, {
      headers: headers,
      body: JSON.stringify(body),
      withCredentials: true
    });
  }
}