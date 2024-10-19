import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    CommonModule
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],
})
export class AdminDashboardComponent implements OnInit {
  users: any[] = [];  // To hold the list of users
  isLoading = true;   // To show a loading indicator while fetching data
  isError = false;    // Flag for handling errors
  errorMessage: string = '';

  constructor(
    private http: HttpClient,
    private router: Router,
    private cookieService: CookieService
  ) {}

  ngOnInit(): void {
    this.fetchUsers();  // Fetch users when the component is initialized
  }

  fetchUsers() {
    // Retrieve the JWT token from the cookies
    const token = this.cookieService.get('accessToken');

    // Set the headers with the Bearer token
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    // Make the GET request to fetch users
    this.http.get('https://localhost:7292/user', { headers }).subscribe({
      next: (response: any) => {
        this.users = response;  // Store the fetched users
        this.isLoading = false;  // Set loading to false
      },
      error: (err) => {
        console.error('Error fetching users', err);
        this.isError = true;  // Set error flag
        this.errorMessage = 'There was an error fetching the users.';
        this.isLoading = false;
      }
    });
  }

  // Function to delete a user
  deleteUser(userId: number) {
    const token = this.cookieService.get('accessToken');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    this.http.delete(`https://localhost:7292/user/${userId}`, { headers }).subscribe({
      next: (response: any) => {
        console.log(`User with ID ${userId} deleted successfully.`);
        this.fetchUsers();  // Refresh the list after deletion
      },
      error: (err) => {
        console.error(`Error deleting user with ID ${userId}`, err);
        this.isError = true;
        this.errorMessage = `Error deleting user with ID ${userId}.`;
      }
    });
  }

  // Function to navigate to the edit user page (You need to create this page separately)
  editUser(userId: number) {
    this.router.navigate([`/user/edit/${userId}`]);  // Navigate to the edit user page
  }
}
