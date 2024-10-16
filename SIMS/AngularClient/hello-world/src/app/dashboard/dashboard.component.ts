import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="dashboard-container">
      <h2>Dashboard</h2>
      
      <!-- Ticket Panel -->
      <div class="panel" (click)="goToTickets()">
        <h3>Tickets</h3>
        <p>Click here to view your tickets.</p>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
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
export class DashboardComponent {
  constructor(private router: Router) {}

  goToTickets() {
    this.router.navigate(['/tickets']);
  }
}
