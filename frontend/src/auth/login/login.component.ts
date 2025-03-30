import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgClass, NgIf } from '@angular/common';
import { AuthGatewayService } from '../auth-gateway.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, NgClass, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  submitted = false;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, private authGateway: AuthGatewayService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  get formControls() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = null;

    if (this.loginForm.invalid) return;

    const { email, password } = this.loginForm.value;

    this.authGateway.login(email, password).subscribe({
      next: (response) => {
        console.log('Login successful:', response);
        this.loginForm.reset();
        this.submitted = false;
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.errorMessage = 'Login failed. Please check your credentials and try again.';
      },
    });
  }
}