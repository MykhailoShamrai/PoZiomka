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

interface NewRoom {
  floor: number;
  number: number;
  capacity: number;
  status: RoomStatus;
}

interface ChangeRoomStatusRequest {
  roomId: number;
  status: RoomStatus;
}

@Component({
  selector: 'app-room-management',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './room-management.component.html',
  styleUrl: './room-management.component.css'
})
export class RoomManagementComponent implements OnInit {
  rooms: Room[] = [];
  filteredRooms: Room[] = [];
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  
  // Filters
  floorFilter: number | null = null;
  capacityFilter: number | null = null;
  statusFilter: RoomStatus | null = null;
  availabilityFilter: boolean | null = null;
  
  // New room form
  newRoom: NewRoom = {
    floor: 1,
    number: 0,
    capacity: 2,
    status: RoomStatus.Available
  };
  
  // Room to delete
  roomToDelete: Room | null = null;
  
  // Room to change status
  roomToChangeStatus: Room | null = null;
  newStatus: RoomStatus = RoomStatus.Available;
  
  // Status options for display
  roomStatuses = [
    { value: RoomStatus.Available, label: 'Available' },
    { value: RoomStatus.Unavailable, label: 'Unavailable' },
    { value: RoomStatus.Renovation, label: 'Under Renovation' },
    { value: RoomStatus.Cleaning, label: 'Cleaning' }
  ];
  
  constructor(
    private http: HttpClient,
    private authService: AuthGatewayService
  ) {}

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.loading = true;
    this.errorMessage = null;
    
    this.http.get<Room[]>(`${environment.apiUrl}Admin/get_all_rooms`)
      .subscribe({
        next: (data) => {
          this.rooms = data;
          this.applyFilters();
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading rooms:', err);
          this.errorMessage = 'Failed to load rooms. Please try again.';
          this.loading = false;
        }
      });
  }

  applyFilters(): void {
    this.filteredRooms = this.rooms.filter(room => {
      // Apply floor filter
      if (this.floorFilter !== null && room.floor !== this.floorFilter) {
        return false;
      }
      
      // Apply capacity filter
      if (this.capacityFilter !== null && room.capacity !== this.capacityFilter) {
        return false;
      }
      
      // Apply status filter
      if (this.statusFilter !== null && room.status !== this.statusFilter) {
        return false;
      }
      
      // Apply availability filter
      if (this.availabilityFilter !== null) {
        if (this.availabilityFilter && room.freePlaces === 0) {
          return false;
        }
        if (!this.availabilityFilter && room.freePlaces > 0) {
          return false;
        }
      }
      
      return true;
    });
  }

  resetFilters(): void {
    this.floorFilter = null;
    this.capacityFilter = null;
    this.statusFilter = null;
    this.availabilityFilter = null;
    this.applyFilters();
  }

  getUniqueFloors(): number[] {
    const floors = new Set<number>();
    this.rooms.forEach(room => floors.add(room.floor));
    return Array.from(floors).sort((a, b) => a - b);
  }

  getUniqueCapacities(): number[] {
    const capacities = new Set<number>();
    this.rooms.forEach(room => capacities.add(room.capacity));
    return Array.from(capacities).sort((a, b) => a - b);
  }

  getRoomStatusClass(status: RoomStatus): string {
    switch (status) {
      case RoomStatus.Available: return 'text-success';
      case RoomStatus.Unavailable: return 'text-danger';
      case RoomStatus.Renovation: return 'text-warning';
      case RoomStatus.Cleaning: return 'text-info';
      default: return '';
    }
  }

  getRoomStatusText(status: RoomStatus): string {
    switch(status) {
      case RoomStatus.Available: return 'Available';
      case RoomStatus.Unavailable: return 'Unavailable';
      case RoomStatus.Renovation: return 'Under Renovation';
      case RoomStatus.Cleaning: return 'Cleaning';
      default: return 'Unknown';
    }
  }

  addRoom(): void {
    this.loading = true;
    this.errorMessage = null;
    this.successMessage = null;
    
    this.http.post(`${environment.apiUrl}Admin/add_new_room`, [this.newRoom])
      .subscribe({
        next: () => {
          this.successMessage = `Room ${this.newRoom.number} on floor ${this.newRoom.floor} added successfully.`;
          // Reset the form
          this.newRoom = {
            floor: 1,
            number: 0,
            capacity: 2,
            status: RoomStatus.Available
          };
          // Reload rooms
          this.loadRooms();
        },
        error: (err) => {
          console.error('Error adding room:', err);
          this.errorMessage = 'Failed to add room. Please try again.';
          this.loading = false;
        }
      });
  }

  confirmDelete(room: Room): void {
    this.roomToDelete = room;
  }

  cancelDelete(): void {
    this.roomToDelete = null;
  }

  deleteRoom(): void {
    if (!this.roomToDelete) return;
    
    this.loading = true;
    this.errorMessage = null;
    this.successMessage = null;
    
    const roomData = {
      floor: this.roomToDelete.floor,
      number: this.roomToDelete.number,
      capacity: this.roomToDelete.capacity,
      status: this.roomToDelete.status
    };
    
    this.http.delete(`${environment.apiUrl}Admin/delete_room`, { 
      body: roomData
    }).subscribe({
      next: () => {
        this.successMessage = `Room ${this.roomToDelete?.number} on floor ${this.roomToDelete?.floor} deleted successfully.`;
        this.roomToDelete = null;
        // Reload rooms
        this.loadRooms();
      },
      error: (err) => {
        console.error('Error deleting room:', err);
        this.errorMessage = 'Failed to delete room. Please try again.';
        this.loading = false;
        this.roomToDelete = null;
      }
    });
  }

  changeRoomStatus(): void {
    if (!this.roomToChangeStatus) return;
    
    this.loading = true;
    this.errorMessage = null;
    this.successMessage = null;
    
    const request: ChangeRoomStatusRequest = {
      roomId: this.roomToChangeStatus.id,
      status: this.newStatus
    };
    
    this.http.put(`${environment.apiUrl}Admin/set_status_to_room`, request)
      .subscribe({
        next: () => {
          this.successMessage = `Status for Room ${this.roomToChangeStatus?.number} changed to ${this.getRoomStatusText(this.newStatus)}.`;
          this.roomToChangeStatus = null;
          // Reload rooms
          this.loadRooms();
        },
        error: (err) => {
          console.error('Error changing room status:', err);
          this.errorMessage = 'Failed to change room status. Please try again.';
          this.loading = false;
          this.roomToChangeStatus = null;
        }
      });
  }
  
  confirmChangeStatus(room: Room): void {
    this.roomToChangeStatus = room;
    this.newStatus = room.status;
  }
  
  cancelChangeStatus(): void {
    this.roomToChangeStatus = null;
  }
  
  logout(): void {
    this.authService.logout();
  }
}