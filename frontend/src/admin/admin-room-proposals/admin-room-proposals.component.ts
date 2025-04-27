import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthGatewayService } from '../../auth/auth-gateway.service';
import { FormsModule } from '@angular/forms';

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
  status?: string;
}

interface RoomProposal {
  id: number;
  status: 'Pending' | 'Accepted' | 'Rejected' | 'AcceptedByAdmin' | 'RejectedByAdmin';
  room: Room;
  potentialRoommates: Student[];
  createdAt: Date;
  acceptedByAllUsers: boolean;
}

@Component({
  selector: 'app-admin-room-proposals',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './admin-room-proposals.component.html',
  styleUrl: './admin-room-proposals.component.css'
})
export class AdminRoomProposalsComponent implements OnInit {
  proposals: RoomProposal[] = [];
  pendingProposals: RoomProposal[] = [];
  approvedProposals: RoomProposal[] = [];
  rejectedProposals: RoomProposal[] = [];
  
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  
  filterStatus: string = 'all';
  
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
    // this.http.get<RoomProposal[]>(`${environment.apiUrl}admin/roomProposals`)
    //   .subscribe({ ... });
    
    // Using mock data for now
    setTimeout(() => {
      this.proposals = this.getMockProposals();
      this.filterProposals();
      this.loading = false;
    }, 1000);
  }

  filterProposals(): void {
    this.pendingProposals = this.proposals.filter(p => 
      p.status === 'Pending' || p.status === 'Accepted');
    
    this.approvedProposals = this.proposals.filter(p => 
      p.status === 'AcceptedByAdmin');
    
    this.rejectedProposals = this.proposals.filter(p => 
      p.status === 'Rejected' || p.status === 'RejectedByAdmin');
  }

  approveProposal(proposal: RoomProposal): void {
    this.loading = true;
    
    // In a real implementation:
    // this.http.patch(`${environment.apiUrl}admin/match-proposals/${proposal.id}/status`, { status: 'AcceptedByAdmin' })
    //   .subscribe({ ... });
    
    // Mock implementation
    setTimeout(() => {
      proposal.status = 'AcceptedByAdmin';
      this.successMessage = `Proposal for Room ${proposal.room.number} approved successfully.`;
      this.loading = false;
      this.filterProposals();
      
      // Clear success message after 3 seconds
      setTimeout(() => this.successMessage = null, 3000);
    }, 1000);
  }

  rejectProposal(proposal: RoomProposal): void {
    this.loading = true;
    
    // In a real implementation:
    // this.http.patch(`${environment.apiUrl}admin/match-proposals/${proposal.id}/status`, { status: 'RejectedByAdmin' })
    //   .subscribe({ ... });
    
    // Mock implementation
    setTimeout(() => {
      proposal.status = 'RejectedByAdmin';
      this.successMessage = `Proposal for Room ${proposal.room.number} rejected.`;
      this.loading = false;
      this.filterProposals();
      
      // Clear success message after 3 seconds
      setTimeout(() => this.successMessage = null, 3000);
    }, 1000);
  }

  getMockProposals(): RoomProposal[] {
    return [
      {
        id: 1,
        status: 'Accepted',
        room: {
          id: 101,
          number: '101',
          floor: 1,
          capacity: 2,
          currentOccupancy: 1,
          status: 'Available'
        },
        potentialRoommates: [
          {
            id: 1,
            email: 'john.doe@example.com',
            firstName: 'John',
            lastName: 'Doe'
          },
          {
            id: 2,
            email: 'anna.kowalska@example.com',
            firstName: 'Anna',
            lastName: 'Kowalska'
          }
        ],
        createdAt: new Date('2025-03-15'),
        acceptedByAllUsers: true
      },
      {
        id: 2,
        status: 'AcceptedByAdmin',
        room: {
          id: 203,
          number: '203',
          floor: 2,
          capacity: 3,
          currentOccupancy: 3,
          status: 'Occupied'
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
          },
          {
            id: 5,
            email: 'piotr.kowalczyk@example.com',
            firstName: 'Piotr',
            lastName: 'Kowalczyk'
          }
        ],
        createdAt: new Date('2025-03-10'),
        acceptedByAllUsers: true
      },
      {
        id: 3,
        status: 'Pending',
        room: {
          id: 305,
          number: '305',
          floor: 3,
          capacity: 2,
          currentOccupancy: 0,
          status: 'Available'
        },
        potentialRoommates: [
          {
            id: 6,
            email: 'adam.wisniewski@example.com',
            firstName: 'Adam',
            lastName: 'Wiśniewski'
          },
          {
            id: 7,
            email: 'karolina.lewandowska@example.com',
            firstName: 'Karolina',
            lastName: 'Lewandowska'
          }
        ],
        createdAt: new Date('2025-03-18'),
        acceptedByAllUsers: false
      },
      {
        id: 4,
        status: 'Accepted',
        room: {
          id: 402,
          number: '402',
          floor: 4,
          capacity: 3,
          currentOccupancy: 1,
          status: 'Available'
        },
        potentialRoommates: [
          {
            id: 8,
            email: 'michal.kaminski@example.com',
            firstName: 'Michał',
            lastName: 'Kamiński'
          },
          {
            id: 9,
            email: 'aleksandra.zielinska@example.com',
            firstName: 'Aleksandra',
            lastName: 'Zielińska'
          },
          {
            id: 10,
            email: 'tomasz.szymanski@example.com',
            firstName: 'Tomasz',
            lastName: 'Szymański'
          }
        ],
        createdAt: new Date('2025-03-17'),
        acceptedByAllUsers: true
      },
      {
        id: 5,
        status: 'RejectedByAdmin',
        room: {
          id: 501,
          number: '501',
          floor: 5,
          capacity: 2,
          currentOccupancy: 0,
          status: 'Under maintenance'
        },
        potentialRoommates: [
          {
            id: 11,
            email: 'natalia.wojcik@example.com',
            firstName: 'Natalia',
            lastName: 'Wójcik'
          },
          {
            id: 12,
            email: 'kamil.kowalski@example.com',
            firstName: 'Kamil',
            lastName: 'Kowalski'
          }
        ],
        createdAt: new Date('2025-03-12'),
        acceptedByAllUsers: true
      }
    ];
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Pending': return 'text-warning';
      case 'Accepted': return 'text-primary';
      case 'Rejected': return 'text-danger';
      case 'RejectedByAdmin': return 'text-danger';
      case 'AcceptedByAdmin': return 'text-success';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'Pending': return 'Pending user response';
      case 'Accepted': return 'Accepted by users (waiting for your approval)';
      case 'Rejected': return 'Rejected by at least one user';
      case 'RejectedByAdmin': return 'Rejected by administrator';
      case 'AcceptedByAdmin': return 'Approved (final)';
      default: return status;
    }
  }

  canApproveOrReject(proposal: RoomProposal): boolean {
    return proposal.status === 'Accepted' && proposal.acceptedByAllUsers;
  }

  getRoomStatusClass(status: string): string {
    switch (status) {
      case 'Available': return 'text-success';
      case 'Occupied': return 'text-danger';
      case 'Under maintenance': return 'text-warning';
      default: return '';
    }
  }

  logout(): void {
    this.authService.logout();
  }
}