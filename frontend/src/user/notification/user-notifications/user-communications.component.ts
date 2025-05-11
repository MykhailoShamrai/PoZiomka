import { Component } from '@angular/core';
import { UserService } from '../../user.service';
import { CommonModule } from '@angular/common';

export interface Communication {
  id: number;
  userId: string;
  type: number;
  description: string;
}

@Component({
  selector: 'app-user-communications',
  imports: [CommonModule],
  templateUrl: './user-communications.component.html',
  styleUrl: './user-communications.component.css'
})
export class UserCommunicationsComponent {
  communications: Communication[] = [];

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.userService.fetchCurrentUserCommunications().subscribe({
      next: (data) => {
        this.communications = data;
      },
      error: () => {
        console.error('Failed to load notifications');
        this.communications = [];
      }
    });
  }
}
