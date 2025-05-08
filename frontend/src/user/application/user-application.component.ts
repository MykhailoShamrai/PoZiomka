import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-user-application',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-application.component.html',
  styleUrls: ['./user-application.component.css']
})
export class UserApplicationComponent {
  applicationText: string = '';
  isSubmitting: boolean = false;
  submitSuccess: boolean = false;
  submitError: string | null = null;

  constructor(private http: HttpClient) {}

  submitApplication(): void {
    if (!this.applicationText.trim()) {
      this.submitError = 'Please enter your question or application text.';
      return;
    }

    this.isSubmitting = true;
    this.submitError = null;

    this.http.post('/api/user/submit-application', { text: this.applicationText })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.submitSuccess = true;
          this.applicationText = '';
          setTimeout(() => this.submitSuccess = false, 3000);
        },
        error: (error) => {
          this.isSubmitting = false;
          this.submitError = 'Failed to submit your application. Please try again later.';
          console.error('Application submission error:', error);
        }
      });
  }
}
