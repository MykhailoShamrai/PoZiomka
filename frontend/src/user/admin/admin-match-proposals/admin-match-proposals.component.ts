import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthGatewayService } from '../../../auth/auth-gateway.service';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environment/environment';

// Enums to match API response
enum RoomStatus {
  Available = 0,
  Occupied = 1,
  UnderMaintenance = 2
}

enum SingleStudentStatus {
  Accepted = 0,
  Rejected = 1,
  Pending = 2
}

enum AdminStatus {
  Accepted = 0,
  Rejected = 1,
  Pending = 2,
  Unavailable = 3
}

enum StatusOfProposal {
  WaitingForRoommates = 0,
  AcceptedByRoommates = 1,
  RejectedByOneOrMoreUsers = 2,
  AcceptedByAdmin = 3,
  RejectedByAdmin = 4
}

interface Student {
  id: number;
  email: string;
  name: string;
  surname: string;
  phoneNumber?: string;
}

interface Room {
  id: number;
  number: number;
  floor: number;
  capacity: number;
  status: RoomStatus;
  residentsIds: number[];
  freePlaces: number;
}

interface MatchProposal {
  id: number;
  room: Room;
  roommatesIds: number[];
  roommates: Student[];
  statuses: SingleStudentStatus[];
  adminStatus: AdminStatus;
  timestamp: string;
  wholeStatus: StatusOfProposal;
}

@Component({
  selector: 'app-admin-match-proposals',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './admin-match-proposals.component.html',
  styleUrl: './admin-match-proposals.component.css'
})
export class AdminMatchProposalsComponent implements OnInit {
  proposals: MatchProposal[] = [];
  pendingProposals: MatchProposal[] = [];
  approvedProposals: MatchProposal[] = [];
  rejectedProposals: MatchProposal[] = [];
  
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
    
    this.http.get<MatchProposal[]>(`${environment.apiUrl}Admin/get_all_proposals`)
      .subscribe({
        next: (data) => {
          this.proposals = data;
          this.filterProposals();
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading match proposals:', err);
          this.errorMessage = 'Failed to load match proposals. Please try again.';
          this.loading = false;
        }
      });
  }

  filterProposals(): void {
    this.pendingProposals = [...this.proposals];
    this.approvedProposals = [...this.proposals];
    this.rejectedProposals = [...this.proposals];
  }

  approveProposal(proposal: MatchProposal): void {
    this.loading = true;
    
    this.http.post(`${environment.apiUrl}Admin/approve_proposal/${proposal.id}`, {})
      .subscribe({
        next: () => {
          proposal.wholeStatus = StatusOfProposal.AcceptedByAdmin;
          proposal.adminStatus = AdminStatus.Accepted;
          this.successMessage = `Proposal for Room ${proposal.room.number} approved successfully.`;
          this.loading = false;
          this.filterProposals();
          setTimeout(() => this.successMessage = null, 3000);
        },
        error: (err) => {
          console.error('Error approving proposal:', err);
          this.errorMessage = 'Failed to approve proposal. Please try again.';
          this.loading = false;
        }
      });
  }

  rejectProposal(proposal: MatchProposal): void {
    this.loading = true;
    
    this.http.post(`${environment.apiUrl}Admin/reject_proposal/${proposal.id}`, {})
      .subscribe({
        next: () => {
          proposal.wholeStatus = StatusOfProposal.RejectedByAdmin;
          proposal.adminStatus = AdminStatus.Rejected;
          this.successMessage = `Proposal for Room ${proposal.room.number} rejected.`;
          this.loading = false;
          this.filterProposals();
          setTimeout(() => this.successMessage = null, 3000);
        },
        error: (err) => {
          console.error('Error rejecting proposal:', err);
          this.errorMessage = 'Failed to reject proposal. Please try again.';
          this.loading = false;
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

  getStatusText(status: StatusOfProposal, statuses: SingleStudentStatus[]): string {
    switch (status) {
      case StatusOfProposal.WaitingForRoommates: 
        return 'Waiting for student responses';
      case StatusOfProposal.AcceptedByRoommates: 
        return 'Accepted by all users (waiting for your approval)';
      case StatusOfProposal.RejectedByOneOrMoreUsers: 
        return 'Rejected by at least one user';
      case StatusOfProposal.RejectedByAdmin: 
        return 'Rejected by administrator';
      case StatusOfProposal.AcceptedByAdmin: 
        return 'Approved (final)';
      default: 
        return 'Unknown status';
    }
  }

  canApproveOrReject(proposal: MatchProposal): boolean {
    return proposal.wholeStatus === StatusOfProposal.AcceptedByRoommates;
  }

  getRoomStatusClass(status: RoomStatus): string {
    switch (status) {
      case RoomStatus.Available: return 'text-success';
      case RoomStatus.Occupied: return 'text-danger';
      case RoomStatus.UnderMaintenance: return 'text-warning';
      default: return '';
    }
  }

  getRoomStatusText(status: RoomStatus): string {
    switch(status) {
      case RoomStatus.Available: return 'Available';
      case RoomStatus.Occupied: return 'Occupied';
      case RoomStatus.UnderMaintenance: return 'Under Maintenance';
      default: return 'Unknown';
    }
  }

  logout(): void {
    this.authService.logout();
  }
}