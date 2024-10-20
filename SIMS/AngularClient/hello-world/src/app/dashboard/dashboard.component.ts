import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';  // Import CommonModule to use *ngIf
import { jwtDecode } from 'jwt-decode';  // Correct import for jwtDecode

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],  // Add CommonModule to the imports array
  template: `
    <div class="dashboard-container">
      <h2>Dashboard</h2>

      <!-- Ticket Panel -->
      <div class="panel" (click)="goToTickets()">
        <h3>Tickets</h3>
        <p>Click here to view your tickets.</p>
      </div>

      <!-- Admin Dashboard Panel (Visible only if the user is an admin) -->
      <div class="panel" *ngIf="isAdmin" (click)="goToAdminDashboard()">
        <h3>Admin Dashboard</h3>
        <p>Click here to access the admin dashboard.</p>
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
export class DashboardComponent implements OnInit {
  isAdmin: boolean = false;  // To check if the user is an admin

  apiUrl = (window as any).__env?.WEB_API_URL;

  constructor(private router: Router, private cookieService: CookieService, private http: HttpClient) {}

  ngOnInit() {
    this.checkUserRole();
  }

  // Function to check the user's role by fetching it from the API
  checkUserRole() {
    const token = this.cookieService.get('accessToken');

    if (token) {
      const decodedToken: any = jwtDecode(token);  // Decode the token
      const userId = decodedToken.sub;  // Extract the user ID (sub) from the decoded token

      // Prepare headers with the token
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      });

      // Make an API call to /user/{userId} to get the user's details
      this.http.get(`${this.apiUrl}/user/${userId}`, { headers }).subscribe({
        next: (response: any) => {
          // Check if the user has the admin role (role === 2)
          this.isAdmin = response && response.role === 2;
        },
        error: (err) => {
          console.error('Error fetching user details:', err);
        }
      });
    }
  }

  // Navigate to Tickets
  goToTickets() {
    this.router.navigate(['/tickets']);
  }

  // Navigate to Admin Dashboard (only if the user is an admin)
  goToAdminDashboard() {
    if (this.isAdmin) {
      this.router.navigate(['/admin-dashboard']);
    }
  }
}
