import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environment/environment';

enum ApplicationStatus {
  Sent = 0,
  Considered = 1,
}

interface Application {
  applicationId: number;
  userId: number;
  description: string;
  status: ApplicationStatus;
  answer?: string;
}

interface ApplicationAnswer {
  applicationAnswerId: number;
  description: string;
}

@Component({
  selector: 'app-user-application',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-application.component.html',
  styleUrls: ['./user-application.component.css']
})
export class UserApplicationComponent implements OnInit {
  applicationText: string = '';
  isSubmitting: boolean = false;
  submitSuccess: boolean = false;
  submitError: string | null = null;

  applications: Application[] = [];
  loading: boolean = false;
  errorMessage: string | null = null;
  ApplicationStatus = ApplicationStatus;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.loading = true;
    this.errorMessage = null;

    this.http.get<Application[]>(`${environment.apiUrl}user/my_applications`)
      .subscribe({
        next: (data) => {
          this.applications = data.map(app => ({
            applicationId: app.applicationId,
            userId: app.userId,
            description: app.description || 'No description provided',
            status: app.status,
            answer: undefined
          }));
          this.loading = false;
          this.fetchApplicationAnswers();
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = 'Failed to load your applications. Please try again later.';
          console.error('Error loading applications:', error);
        }
      });
  }

  fetchApplicationAnswers(): void {
    this.applications.forEach((app, index) => {
      if (app.status === ApplicationStatus.Considered) {
        this.http.get<ApplicationAnswer>(`${environment.apiUrl}user/answer_for_application?applicationId=${app.applicationId}`)
          .subscribe({
            next: (answer) => {
              this.applications[index].answer = answer.description;
            },
            error: (error) => {
              console.error(`Error fetching answer for application ${app.applicationId}:`, error);
              this.applications[index].answer = 'Error fetching response';
            }
          });
      }
    });
  }

  submitApplication(): void {
    if (!this.applicationText.trim()) {
      this.submitError = 'Please enter your question or application text.';
      return;
    }

    this.isSubmitting = true;
    this.submitError = null;

    this.http.post(`${environment.apiUrl}user/send_application`, { description: this.applicationText })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.submitSuccess = true;
          this.applicationText = '';
          setTimeout(() => this.submitSuccess = false, 3000);
          this.loadApplications();
        },
        error: (error) => {
          this.isSubmitting = false;
          this.submitError = 'Failed to submit your application. Please try again later.';
          console.error('Application submission error:', error);
        }
      });
  }

  getStatusLabel(status: ApplicationStatus): string {
    switch (status) {
      case ApplicationStatus.Sent:
        return 'Sent';
      case ApplicationStatus.Considered:
        return 'Considered';
      default:
        return 'Unknown';
    }
  }
}
