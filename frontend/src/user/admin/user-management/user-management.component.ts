import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthGatewayService } from '../../../auth/auth-gateway.service';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environment/environment';

enum RoomStatus {
  Available = 0,
  Unavailable = 1,
  Renovation = 2,
  Cleaning = 3
}

interface Room {
  id: number;
  floor: number;
  number: number;
  capacity: number;
  status: RoomStatus;
  residentsIds: number[];
  freePlaces: number;
}

interface UserRoomRequest {
  userEmail: string;
  roomId: number;
}

interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  roomId?: number;
  roomNumber?: number;
}

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  rooms: Room[] = [];
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  
  // User filters
  searchTerm: string = '';
  
  // User room assignment
  userEmail: string = '';
  selectedRoomForUser: number | null = null;
  addingUser: boolean = false;
  removingUser: boolean = false;
  
  constructor(
    private http: HttpClient,
    private authService: AuthGatewayService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadRooms();
  }

  loadUsers(): void {
    this.loading = true;
    
    // TO DO
    this.http.get<User[]>(`${environment.apiUrl}Admin/get_all_users`)
      .subscribe({
        next: (data) => {
          this.users = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading users:', err);
          this.errorMessage = 'Failed to load users. Please try again.';
          this.loading = false;
        }
      });
  }

  loadRooms(): void {
    this.http.get<Room[]>(`${environment.apiUrl}Admin/get_all_rooms`)
      .subscribe({
        next: (data) => {
          this.rooms = data;
        },
        error: (err) => {
          console.error('Error loading rooms:', err);
        }
      });
  }

  get filteredUsers(): User[] {
    if (!this.searchTerm) {
      return this.users;
    }
    
    const term = this.searchTerm.toLowerCase();
    return this.users.filter(user => 
      user.email.toLowerCase().includes(term) ||
      (user.firstName && user.firstName.toLowerCase().includes(term)) ||
      (user.lastName && user.lastName.toLowerCase().includes(term))
    );
  }

  addUserToRoom(): void {
    if (!this.selectedRoomForUser || !this.userEmail) return;
    
    this.loading = true;
    this.errorMessage = null;
    this.successMessage = null;
    
    const request: UserRoomRequest = {
      userEmail: this.userEmail,
      roomId: this.selectedRoomForUser
    };
    
    this.http.put(`${environment.apiUrl}Admin/add_user_to_room`, request)
      .subscribe({
        next: () => {
          this.successMessage = `User ${this.userEmail} assigned to Room ${this.rooms.find(r => r.id === this.selectedRoomForUser)?.number}.`;
          this.userEmail = '';
          this.selectedRoomForUser = null;
          this.addingUser = false;
          // Reload data
          this.loadUsers();
          this.loadRooms();
        },
        error: (err) => {
          console.error('Error adding user to room:', err);
          this.errorMessage = 'Failed to add user to room. Please try again.';
          this.loading = false;
        }
      });
  }
  
  removeUserFromRoom(): void {
    if (!this.selectedRoomForUser || !this.userEmail) return;
    
    this.loading = true;
    this.errorMessage = null;
    this.successMessage = null;
    
    const request: UserRoomRequest = {
      userEmail: this.userEmail,
      roomId: this.selectedRoomForUser
    };
    
    this.http.put(`${environment.apiUrl}Admin/remove_user_from_room`, request)
      .subscribe({
        next: () => {
          this.successMessage = `User ${this.userEmail} removed from Room ${this.rooms.find(r => r.id === this.selectedRoomForUser)?.number}.`;
          this.userEmail = '';
          this.selectedRoomForUser = null;
          this.removingUser = false;
          // Reload data
          this.loadUsers();
          this.loadRooms();
        },
        error: (err) => {
          console.error('Error removing user from room:', err);
          this.errorMessage = 'Failed to remove user from room. Please try again.';
          this.loading = false;
        }
      });
  }
  
  showAddUserForm(): void {
    this.addingUser = true;
    this.removingUser = false;
    this.userEmail = '';
    this.selectedRoomForUser = null;
  }
  
  showRemoveUserForm(): void {
    this.removingUser = true;
    this.addingUser = false;
    this.userEmail = '';
    this.selectedRoomForUser = null;
  }
  
  cancelUserOperation(): void {
    this.addingUser = false;
    this.removingUser = false;
    this.userEmail = '';
    this.selectedRoomForUser = null;
  }
  
  removeUser(user: User): void {
    if (!user.roomId) return;
    
    this.userEmail = user.email;
    this.selectedRoomForUser = user.roomId;
    this.removeUserFromRoom();
  }
  
  getRoomForUser(userId: number): string {
    const user = this.users.find(u => u.id === userId);
    if (!user || !user.roomNumber) return 'Not assigned';
    return `Room ${user.roomNumber}`;
  }
  
  logout(): void {
    this.authService.logout();
  }
}