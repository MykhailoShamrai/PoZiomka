import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthGatewayService } from '../../../auth/auth-gateway.service';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environment/environment';

enum ApplicationStatus {
  Sent = 0,
  Considered = 1,
}

interface User {
  id: number;
  email: string;
}

interface Application {
  applicationId: number;
  userId: number;
  description: string;
  status: ApplicationStatus;
  userEmail?: string;
}

interface UpdateApplicationRequest {
  applicationId: number;
  status: ApplicationStatus;
  description: string;
}

@Component({
  selector: 'app-application-management',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './admin-application.component.html',
  styleUrl: './admin-application.component.css'
})
export class AdminApplicationComponent implements OnInit {
  applications: Application[] = [];
  filteredApplications: Application[] = [];
  users: User[] = [];
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  searchTerm: string = '';
  ApplicationStatus = ApplicationStatus;

  selectedApplicationId: number | null = null;
  responseText: string = '';
  actionType: 'approve' | 'reject' | null = null;

  constructor(
    private http: HttpClient,
    private authService: AuthGatewayService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadApplications();
  }

  loadUsers(): void {
    this.http.get<User[]>(`${environment.apiUrl}Admin/users_information`)
      .subscribe({
        next: (data) => {
          this.users = data;
          this.mapUserEmailsToApplications();
        },
        error: (err) => {
          console.error('Error loading users:', err);
          this.errorMessage = 'Failed to load user information. Emails may not display correctly.';
        }
      });
  }

  loadApplications(): void {
    this.loading = true;
    this.http.get<Application[]>(`${environment.apiUrl}Admin/get_all_applications`)
      .subscribe({
        next: (data) => {
          this.applications = data;
          this.filteredApplications = [...this.applications];
          this.loading = false;
          this.mapUserEmailsToApplications();
        },
        error: (err) => {
          console.error('Error loading applications:', err);
          this.errorMessage = 'Failed to load applications. Please try again.';
          this.loading = false;
        }
      });
  }

  mapUserEmailsToApplications(): void {
    if (this.users.length === 0 || this.applications.length === 0) return;
    this.applications = this.applications.map(app => ({
      ...app,
      userEmail: this.users.find(user => user.id === app.userId)?.email
    }));
    this.filteredApplications = [...this.applications];
  }

  filterApplications(): void {
    if (!this.searchTerm) {
      this.filteredApplications = [...this.applications];
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredApplications = this.applications.filter(app =>
      (app.userEmail && app.userEmail.toLowerCase().includes(term)) ||
      app.description.toLowerCase().includes(term)
    );
  }

  showResponseForm(application: Application, action: 'approve' | 'reject'): void {
    this.selectedApplicationId = application.applicationId;
    this.actionType = action;
    this.responseText = '';
  }

  cancelResponse(): void {
    this.selectedApplicationId = null;
    this.actionType = null;
    this.responseText = '';
  }

  submitResponse(): void {
    if (!this.selectedApplicationId || !this.actionType || !this.responseText) return;

    const status = ApplicationStatus.Considered;
    this.updateApplicationStatus(this.selectedApplicationId, status, this.responseText);
  }

  updateApplicationStatus(applicationId: number, status: ApplicationStatus, description: string): void {
    this.loading = true;
    this.errorMessage = null;
    this.successMessage = null;

    const request: UpdateApplicationRequest = {
      applicationId: applicationId,
      status: status,
      description: description
    };

    this.http.put(`${environment.apiUrl}Admin/update_application_status`, request)
      .subscribe({
        next: () => {
          this.successMessage = `Application ${applicationId} has been ${this.actionType === 'approve' ? 'approved' : 'rejected'} with a response.`;
          this.selectedApplicationId = null;
          this.actionType = null;
          this.responseText = '';
          this.loadApplications();
        },
        error: (err) => {
          console.error('Error updating application status:', err);
          this.errorMessage = 'Failed to update application status. Please try again.';
          this.loading = false;
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

  logout(): void {
    this.authService.logout();
  }
}
