import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthGatewayService } from '../../auth/auth-gateway.service';
import { environment } from '../../environment/environment';

// Enum dla statusu propozycji zgodnie z API
enum ProposalStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
  AcceptedByAdmin = 3,
  RejectedByAdmin = 4
}

// Enum dla statusu pokoju zgodnie z API
enum RoomStatus {
  Available = 0,
  Occupied = 1,
  UnderMaintenance = 2
}

// Interfejsy dostosowane do formatu API
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
  number: string;
  capacity: number;
  status: RoomStatus;
  residentsIds: number[];
  freePlaces: number;
}

interface RoomProposal {
  id: number;
  room: Room;
  roommates: Roommate[];
  statusOfProposal: ProposalStatus;
  timestamp: string;
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
            proposal.statusOfProposal = ProposalStatus.Accepted;
            this.successMessage = 'Proposal accepted! Waiting for administrator approval.';
          } else {
            proposal.statusOfProposal = ProposalStatus.Rejected;
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

  getStatusClass(status: ProposalStatus): string {
    switch (status) {
      case ProposalStatus.Pending: return 'text-warning';
      case ProposalStatus.Accepted: return 'text-primary';
      case ProposalStatus.Rejected: return 'text-danger';
      case ProposalStatus.RejectedByAdmin: return 'text-danger';
      case ProposalStatus.AcceptedByAdmin: return 'text-success';
      default: return '';
    }
  }

  getStatusText(status: ProposalStatus): string {
    switch (status) {
      case ProposalStatus.Pending: return 'Pending your response';
      case ProposalStatus.Accepted: return 'Accepted (waiting for admin approval)';
      case ProposalStatus.Rejected: return 'Rejected by you';
      case ProposalStatus.RejectedByAdmin: return 'Rejected by administrator';
      case ProposalStatus.AcceptedByAdmin: return 'Approved (final)';
      default: return 'Unknown status';
    }
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
    switch (status) {
      case RoomStatus.Available: return 'Available';
      case RoomStatus.Occupied: return 'Occupied';
      case RoomStatus.UnderMaintenance: return 'Under Maintenance';
      default: return 'Unknown';
    }
  }

  canAcceptOrReject(proposal: RoomProposal): boolean {
    return proposal.statusOfProposal === ProposalStatus.Pending;
  }

  logout(): void {
    this.authService.logout();
  }
}