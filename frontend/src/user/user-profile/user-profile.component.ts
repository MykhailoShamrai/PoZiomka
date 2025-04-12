import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgClass, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../user.service';
import { AuthGatewayService } from '../../auth/auth-gateway.service';

@Component({
  selector: 'app-user-profile',
  imports: [ReactiveFormsModule, NgClass, NgIf],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css',
})
export class UserProfileComponent implements OnInit {
  userForm: FormGroup;
  submitted = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  loading = true;
  editMode = false;
  
  // Default user data in case of API failure
  defaultUserData = {
    email: 'user@example.com',
    firstName: 'John',
    lastName: 'Doe',
    phoneNumber: '555-123-4567'
  };

  constructor(
    private fb: FormBuilder, 
    private userService: UserService,
    private authService: AuthGatewayService
  ) {
    this.userForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: [''],
    });
  }

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    this.loading = true;
    this.userService.getUserProfile().subscribe({
      next: (userData) => {
        console.log(userData);
        this.userForm.patchValue({
          email: userData.email,
          firstName: userData.firstName || '',
          lastName: userData.lastName || '',
          phoneNumber: userData.phoneNumber || '',
        });
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load user data:', err);
        this.errorMessage = 'Failed to load user data. Using default profile.';
        // Load default data in case of API failure
        this.userForm.patchValue(this.defaultUserData);
        this.loading = false;
      },
    });
  }

  get formControls() {
    return this.userForm.controls;
  }

  toggleEditMode(): void {
    this.editMode = !this.editMode;
    if (!this.editMode) {
      // Reset form validation state when exiting edit mode
      this.submitted = false;
    }
  }
  
  logout(): void {
    this.authService.logout();
  }

  onSubmit() {
    this.submitted = true;
    this.errorMessage = null;
    this.successMessage = null;

    if (this.userForm.invalid) return;

    const userData = this.userForm.value;

    this.userService.updateUserProfile(userData).subscribe({
      next: () => {
        this.successMessage = 'Profile updated successfully!';
        this.submitted = false;
        this.toggleEditMode(); // Exit edit mode after successful update
      },
      error: (err) => {
        console.error('Profile update failed:', err);
        this.errorMessage = 'Failed to update profile. Please try again.';
      },
    });
  }
}