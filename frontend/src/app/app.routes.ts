import { Routes } from '@angular/router';
import { RegistrationComponent } from '../auth/registration/registration.component';
import { LoginComponent } from '../auth/login/login.component';
import { UserProfileComponent } from '../user/user-profile/user-profile.component';
import { UserFormsComponent } from '../user/user-forms/user-forms.component';
import { RoomProposalsComponent } from '../user/room-proposals/room-proposals.component';
import { AdminRoomProposalsComponent } from '../admin/admin-room-proposals/admin-room-proposals.component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'register', component: RegistrationComponent },
    { path: 'login', component: LoginComponent },
    { path: 'profile', component: UserProfileComponent },
    { path: 'forms', component: UserFormsComponent },
    { path: 'room-proposals', component: RoomProposalsComponent },
    { path: 'admin/room-proposals', component: AdminRoomProposalsComponent },
];
