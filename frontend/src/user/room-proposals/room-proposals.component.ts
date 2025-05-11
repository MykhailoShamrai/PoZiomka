import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthGatewayService } from '../../auth/auth-gateway.service';
import { environment } from '../../environment/environment';

// Enums to match API response
enum RoomStatus {
  Available = 0,
  Unavailable = 1,
  UnderMaintenance = 2
}

enum UserActionStatus {
  Accepted = 0,
  Rejected = 1,
  Pending = 2
}

enum StatusOfProposal {
  WaitingForRoommates = 0,
  AcceptedByRoommates = 1,
  RejectedByOneOrMoreUsers = 2,
  AcceptedByAdmin = 3,
  RejectedByAdmin = 4,
  Unavailable = 5
}

// Interfaces dostosowane do formatu API
interface Roommate {
  id: number;
  email: string;
  name: string;
  surname: string;
  phoneNumber?: string;
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

interface RoomProposal {
  id: number;
  room: Room;
  roommates: Roommate[];
  statusOfProposal: StatusOfProposal;
  timestamp: string;
  statusForUser: UserActionStatus;
}

@Component({
  selector: 'app-room-proposals',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './room-proposals.component.html',
  styleUrl: './room-proposals.component.css'
})
export class RoomProposalsComponent implements OnInit {
  proposals: RoomProposal[] = [];
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  
  constructor(
    private http: HttpClient,
    private authService: AuthGatewayService
  ) {}

  ngOnInit(): void {
    this.loadProposals();
  }

  loadProposals(): void {
    this.loading = true;
    
    this.http.get<RoomProposal[]>(`${environment.apiUrl}User/get_my_proposals`, { withCredentials: true })
      .subscribe({
        next: (data) => {
          this.proposals = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading room proposals:', err);
          this.errorMessage = 'Failed to load room proposals. Please try again.';
          this.loading = false;
        }
      });
  }

  acceptProposal(proposal: RoomProposal): void {
    this.answerProposal(proposal, 0);
  }

  rejectProposal(proposal: RoomProposal): void {
    this.answerProposal(proposal, 1);
  }

  private answerProposal(proposal: RoomProposal, status: number): void {
    this.loading = true;
    
    const requestBody = {
      proposalId: proposal.id,
      status: status
    };
    
    this.http.put(`${environment.apiUrl}User/answer_prop`, requestBody, { withCredentials: true })
      .subscribe({
        next: () => {
          if (status === 0) {
            proposal.statusForUser = UserActionStatus.Accepted;
            this.successMessage = 'Proposal accepted! Waiting for administrator approval.';
          } else {
            proposal.statusForUser = UserActionStatus.Rejected;
            this.successMessage = 'Proposal rejected successfully.';
          }
          
          this.loading = false;
          
          // Clear success message after 3 seconds
          setTimeout(() => this.successMessage = null, 3000);
        },
        error: (err) => {
          console.error(`Error ${status === 0 ? 'accepting' : 'rejecting'} proposal:`, err);
          this.errorMessage = `Failed to ${status === 0 ? 'accept' : 'reject'} proposal. Please try again.`;
          this.loading = false;
          setTimeout(() => this.errorMessage = null, 3000);
        }
      });
  }

  getStatusClass(status: StatusOfProposal): string {
    switch (status) {
      case StatusOfProposal.WaitingForRoommates: return 'text-warning';
      case StatusOfProposal.AcceptedByRoommates: return 'text-primary';
      case StatusOfProposal.RejectedByOneOrMoreUsers: return 'text-danger';
      case StatusOfProposal.RejectedByAdmin: return 'text-danger';
      case StatusOfProposal.AcceptedByAdmin: return 'text-success';
      default: return '';
    }
  }

  getStatusText(status: StatusOfProposal): string {
    switch (status) {
      case StatusOfProposal.WaitingForRoommates: return 'Waiting for all roommates to respond';
      case StatusOfProposal.AcceptedByRoommates: return 'Accepted (waiting for admin approval)';
      case StatusOfProposal.RejectedByOneOrMoreUsers: return 'Rejected by at least one roommate';
      case StatusOfProposal.RejectedByAdmin: return 'Rejected by administrator';
      case StatusOfProposal.AcceptedByAdmin: return 'Approved (final)';
      case StatusOfProposal.Unavailable: return 'Unavailable';
      default: return 'Unknown status';
    }
  }

  getUserStatusClass(status: UserActionStatus): string {
    switch (status) {
      case UserActionStatus.Accepted: return 'text-success';
      case UserActionStatus.Rejected: return 'text-danger';
      case UserActionStatus.Pending: return 'text-warning';
      default: return '';
    }
  }

  getUserStatusText(status: UserActionStatus): string {
    switch (status) {
      case UserActionStatus.Accepted: return 'You accepted this proposal';
      case UserActionStatus.Rejected: return 'You rejected this proposal';
      case UserActionStatus.Pending: return 'Pending your response';
      default: return 'Unknown status';
    }
  }

  getRoomStatusClass(status: RoomStatus): string {
    switch (status) {
      case RoomStatus.Available: return 'text-success';
      case RoomStatus.Unavailable: return 'text-danger';
      case RoomStatus.UnderMaintenance: return 'text-warning';
      default: return '';
    }
  }

  getRoomStatusText(status: RoomStatus): string {
    switch (status) {
      case RoomStatus.Available: return 'Available';
      case RoomStatus.Unavailable: return 'Unavailable';
      case RoomStatus.UnderMaintenance: return 'Under Maintenance';
      default: return 'Unknown';
    }
  }

  canAcceptOrReject(proposal: RoomProposal): boolean {
    return proposal.statusForUser === UserActionStatus.Pending;
  }

  logout(): void {
    this.authService.logout();
  }
}