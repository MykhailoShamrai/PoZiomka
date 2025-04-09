import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgClass, NgIf } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthGatewayService } from '../auth-gateway.service';

@Component({
  selector: 'app-registration',
  imports: [ReactiveFormsModule, NgClass, NgIf, RouterLink],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css',
})
export class RegistrationComponent {
  registrationForm: FormGroup;
  submitted = false;
  errorMessage: string | null = null;

  // Components are injected automatically if you specify them in constructor
  // Notice @Injectable annotation on AuthGatewayService
  constructor(
    private fb: FormBuilder,
    private authGateway: AuthGatewayService,
    private router: Router
  ) {
    this.registrationForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  get formControls() {
    return this.registrationForm.controls;
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = null;

    if (this.registrationForm.invalid) return;

    const { email, password } = this.registrationForm.value;

    this.authGateway.register(email, password).subscribe({
      next: (response) => {
        console.log('Registration successful:', response);
        this.registrationForm.reset();
        this.submitted = false;
        this.router.navigate(['/profile']);
      },
      error: (err) => {
        console.error('Registration failed:', err);
        this.errorMessage = 'Registration failed. Please try again.';
      },
    });
  }
}
