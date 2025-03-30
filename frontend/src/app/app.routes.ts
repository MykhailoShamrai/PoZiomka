import { Routes } from '@angular/router';
import { RegistrationComponent } from '../auth/registration/registration.component';
import { LoginComponent } from '../auth/login/login.component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'register', component: RegistrationComponent },
    { path: 'login', component: LoginComponent },
];
