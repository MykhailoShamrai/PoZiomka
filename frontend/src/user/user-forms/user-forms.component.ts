import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environment/environment';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Form {
  formId: number;
  nameOfForm: string;
  questions: Question[];
  answers?: Answer;
}

interface Question {
  questionId: number;
  name: string;
  formForWhichCorrespond?: Form;
  options: OptionForQuestion[];
  isObligatory: boolean;
}

interface OptionForQuestion {
  optionForQuestionId: number;
  question: Question;
  name: string;
  answersWhichContains: Answer[];
}

interface Answer {
  formId: number;
  chosenOptionIds: number[];
  status: string;
}

@Component({
  selector: 'app-user-forms',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-forms.component.html',
  styleUrls: ['./user-forms.component.css']
})
export class UserFormsComponent implements OnInit {
  forms: Form[] = [];
  selectedOptions: { [questionId: number]: number } = {};
  openForms: { [key: string]: boolean } = {};
  private apiUrl = `${environment.apiUrl}User`;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadForms();
  }

  toggleForm(formId: number): void {
    this.openForms[formId] = !this.openForms[formId];
  }

  isFormOpen(formId: number): boolean {
    return this.openForms[formId] || false;
  }

  loadForms(): void {
    this.http.get<Form[]>(`${this.apiUrl}/forms`, { withCredentials: true })
      .subscribe({
        next: (forms) => {
          this.forms = forms;
          this.forms.forEach(form => {
            this.openForms[form.formId] = false;

            form.questions.forEach(question => {
              this.selectedOptions[question.questionId] = 0;
            });

            if (form.answers && form.answers.chosenOptionIds?.length > 0) {
              form.questions.forEach(question => {
                const selectedOptionId = form.answers!.chosenOptionIds.find(optionId =>
                  question.options.some(opt => opt.optionForQuestionId === optionId)
                );
                if (selectedOptionId) {
                  this.selectedOptions[question.questionId] = selectedOptionId;
                }
              });
            }
          });
        },
        error: (err) => {
          console.error('Failed to load forms', err);
        }
      });
  }

  onOptionChange(questionId: number, optionId: number): void {
    this.selectedOptions[questionId] = optionId;
  }

  submitForm(form: Form): void {
    const answer: any = {
      formId: form.formId,
      chosenOptionIds: [],
      status: 'Saved'
    };

    form.questions.forEach(question => {
      const selectedOptionId = this.selectedOptions[question.questionId];
      if (selectedOptionId) {
        answer.chosenOptionIds.push(selectedOptionId);
      }
    });

    const unansweredObligatory = form.questions.some(q => q.isObligatory && !this.selectedOptions[q.questionId]);
    if (unansweredObligatory) {
      alert('Please answer all obligatory questions.');
      return;
    }

    this.http.post(`${this.apiUrl}/submit-answer`, answer, { withCredentials: true })
      .subscribe({
        next: () => {
          alert('Form submitted successfully!');
          this.loadForms();
        },
        error: (err) => {
          console.error('Failed to submit form', err);
          alert('Failed to submit form.');
        }
      });
  }
}