import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthGatewayService } from '../../auth/auth-gateway.service';

interface Student {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

interface Room {
  id: number;
  number: string;
  floor: number;
  capacity: number;
  currentOccupancy: number;
}

interface RoomProposal {
  id: number;
  status: 'Pending' | 'Accepted' | 'Rejected' | 'AcceptedByAdmin';
  room: Room;
  potentialRoommates: Student[];
  createdAt: Date;
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
    
    // In a real application, you would fetch from an API
    // this.http.get<RoomProposal[]>(`${environment.apiUrl}roomProposals`)
    //   .subscribe({ ... });
    
    // Using mock data for now
    setTimeout(() => {
      this.proposals = this.getMockProposals();
      this.loading = false;
    }, 1000);
  }

  acceptProposal(proposal: RoomProposal): void {
    this.loading = true;
    
    // In a real application:
    // this.http.patch(`${environment.apiUrl}match-proposals/${proposal.id}/status`, { status: 'Accepted' })
    //   .subscribe({ ... });
    
    // Mock implementation
    setTimeout(() => {
      proposal.status = 'Accepted';
      this.successMessage = 'Proposal accepted! Waiting for administrator approval.';
      this.loading = false;
      
      // Clear success message after 3 seconds
      setTimeout(() => this.successMessage = null, 3000);
    }, 1000);
  }

  rejectProposal(proposal: RoomProposal): void {
    this.loading = true;
    
    // In a real implementation:
    // this.http.patch(`${environment.apiUrl}match-proposals/${proposal.id}/status`, { status: 'Rejected' })
    //   .subscribe({ ... });
    
    // Mock implementation
    setTimeout(() => {
      proposal.status = 'Rejected';
      this.successMessage = 'Proposal rejected successfully.';
      this.loading = false;
      
      // Clear success message after 3 seconds
      setTimeout(() => this.successMessage = null, 3000);
    }, 1000);
  }

  getMockProposals(): RoomProposal[] {
    return [
      {
        id: 1,
        status: 'Pending',
        room: {
          id: 101,
          number: '101',
          floor: 1,
          capacity: 2,
          currentOccupancy: 1
        },
        potentialRoommates: [
          {
            id: 2,
            email: 'anna.kowalska@example.com',
            firstName: 'Anna',
            lastName: 'Kowalska'
          }
        ],
        createdAt: new Date('2025-03-15')
      },
      {
        id: 2,
        status: 'AcceptedByAdmin',
        room: {
          id: 203,
          number: '203',
          floor: 2,
          capacity: 3,
          currentOccupancy: 2
        },
        potentialRoommates: [
          {
            id: 3,
            email: 'jan.nowak@example.com',
            firstName: 'Jan',
            lastName: 'Nowak'
          },
          {
            id: 4,
            email: 'maria.wójcik@example.com',
            firstName: 'Maria',
            lastName: 'Wójcik'
          }
        ],
        createdAt: new Date('2025-03-10')
      },
      {
        id: 3,
        status: 'Accepted',
        room: {
          id: 305,
          number: '305',
          floor: 3,
          capacity: 2,
          currentOccupancy: 1
        },
        potentialRoommates: [
          {
            id: 5,
            email: 'piotr.kowalczyk@example.com',
            firstName: 'Piotr',
            lastName: 'Kowalczyk'
          }
        ],
        createdAt: new Date('2025-03-12')
      }
    ];
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Pending': return 'text-warning';
      case 'Accepted': return 'text-primary';
      case 'Rejected': return 'text-danger';
      case 'AcceptedByAdmin': return 'text-success';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'Pending your response';
      case 'Accepted': return 'Accepted (waiting for admin approval)';
      case 'Rejected': return 'Rejected by you';
      case 'AcceptedByAdmin': return 'Approved (final)';
      default: return status;
    }
  }

  canAcceptOrReject(proposal: RoomProposal): boolean {
    return proposal.status === 'Pending';
  }

  logout(): void {
    this.authService.logout();
  }
}