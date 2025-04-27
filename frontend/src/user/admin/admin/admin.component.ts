import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AddQuestionDto, AdminService, FormCreateDto } from '../admin.service';
import { AuthGatewayService } from '../../../auth/auth-gateway.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent {
  newForm: FormCreateDto = { formName: '' };
  newQuestion: AddQuestionDto = { formName: '', name: '', answers: [''] };
  deleteFormName: string = '';
  deleteQuestionFormName: string = '';
  deleteQuestionName: string = '';
  activeTab: string = 'add-form';

  constructor(
    private adminService: AdminService,
    private authService: AuthGatewayService
  ) {}

  setActiveTab(tabId: string): void {
    this.activeTab = tabId;
  }

  logout(): void {
    this.authService.logout();
  }

  addNewForm(): void {
    if (!this.newForm.formName.trim()) {
      alert('Please enter a form name.');
      return;
    }

    this.adminService.addNewForm(this.newForm).subscribe({
      next: () => {
        alert('Form added successfully!');
        this.newForm.formName = '';
      },
      error: (err) => {
        console.error('Error adding form:', err);
        alert('Failed to add form: ' + (err.error || 'Unknown error'));
      }
    });
  }

  addOption(): void {
    this.newQuestion.answers.push('');
  }

  removeOption(index: number): void {
    if (this.newQuestion.answers.length > 1) {
      this.newQuestion.answers.splice(index, 1);
    }
  }

  addQuestion(): void {
    if (!this.newQuestion.formName.trim() || !this.newQuestion.name.trim() || this.newQuestion.answers.some(opt => !opt.trim())) {
      alert('Please fill in all fields for the question.');
      return;
    }

    this.adminService.addQuestion(this.newQuestion).subscribe({
      next: () => {
        alert('Question added successfully!');
        this.newQuestion = { formName: '', name: '', answers: [''] };
      },
      error: (err) => {
        console.error('Error adding question:', err);
        alert('Failed to add question: ' + (err.error || 'Unknown error'));
      }
    });
  }

  deleteForm(): void {
    if (!this.deleteFormName.trim()) {
      alert('Please enter the name of the form to delete.');
      return;
    }

    this.adminService.deleteForm(this.deleteFormName).subscribe({
      next: () => {
        alert('Form deleted successfully!');
        this.deleteFormName = ''; 
      },
      error: (err) => {
        console.error('Error deleting form:', err);
        alert('Failed to delete form: ' + (err.error || 'Unknown error'));
      }
    });
  }

  deleteQuestion(): void {
    if (!this.deleteQuestionFormName.trim() || !this.deleteQuestionName.trim()) {
      alert('Please enter both the form name and the question name to delete.');
      return;
    }

    this.adminService.deleteQuestion(this.deleteQuestionFormName, this.deleteQuestionName).subscribe({
      next: () => {
        alert('Question deleted successfully!');
        this.deleteQuestionFormName = '';
        this.deleteQuestionName = '';
      },
      error: (err) => {
        console.error('Error deleting question:', err);
        alert('Failed to delete question: ' + (err.error || 'Unknown error'));
      }
    });
  }
}