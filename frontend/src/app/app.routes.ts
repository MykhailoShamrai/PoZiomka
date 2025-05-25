import { Routes } from '@angular/router';
import { RegistrationComponent } from '../auth/registration/registration.component';
import { LoginComponent } from '../auth/login/login.component';
import { UserProfileComponent } from '../user/user-profile/user-profile.component';
import { UserFormsComponent } from '../user/user-forms/user-forms.component';
import { AdminComponent } from '../user/admin/admin/admin.component';
import { RoomProposalsComponent } from '../user/room-proposals/room-proposals.component';
import { AdminMatchProposalsComponent } from '../user/admin/admin-match-proposals/admin-match-proposals.component';
import { UserApplicationComponent } from '../user/application/user-application.component';
import { RoomManagementComponent } from '../user/admin/room-management/room-management.component';
import { UserCommunicationsComponent } from '../user/notification/user-notifications/user-communications.component';
import { UserManagementComponent } from '../user/admin/user-management/user-management.component';
import { AdminApplicationComponent } from '../user/admin/admin-application-component/admin-application.component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'register', component: RegistrationComponent },
    { path: 'login', component: LoginComponent },
    { path: 'profile', component: UserProfileComponent },
    { path: 'forms', component: UserFormsComponent },
    { path: 'admin', component: AdminComponent },
    { path: 'room-proposals', component: RoomProposalsComponent },
    { path: 'admin/match-proposals', component: AdminMatchProposalsComponent },
    { path: 'admin/rooms', component: RoomManagementComponent },
    { path: 'admin/users', component: UserManagementComponent },
    { path: 'admin/applications', component: AdminApplicationComponent },
    { path: 'application', component: UserApplicationComponent },
    { path: 'communications', component: UserCommunicationsComponent },
];
