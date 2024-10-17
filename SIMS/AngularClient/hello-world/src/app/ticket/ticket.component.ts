import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ticket',
  standalone: true,
  template: `
    <div class="ticket-container">
      <h2>Tickets</h2>
      <p>This is where your ticket information will be displayed.</p>
      
      <!-- Ticket Panel -->
      <div class="panel" (click)="goTocreatedTickets()">
        <h3>Created Tickets</h3>
        <p>Click here to view your created tickets.</p>
      </div>

      <!-- Ticket Panel -->
      <div class="panel" (click)="goToassignedTickets()">
        <h3>Assigned Tickets</h3>
        <p>Click here to view your created tickets.</p>
      </div>
    </div>
  `,
  styles: [`
    .ticket-container {
      padding: 20px;
    }
    .panel {
      padding: 20px;
      margin: 10px;
      background-color: #f0f0f0;
      border: 1px solid #ccc;
      cursor: pointer;
      transition: background-color 0.3s ease;
    }
    .panel:hover {
      background-color: #e0e0e0;
    }
  `]
})
export class TicketComponent {
  constructor(private router: Router) {}

  goTocreatedTickets() {
    this.router.navigate(['/created-tickets']);
  }

  goToassignedTickets() {
    this.router.navigate(['/created-tickets']);
  }
}
