import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-assigned-tickets',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './assigned-tickets.component.html',
  styleUrls: ['./assigned-tickets.component.css'],
})
export class AssignedTicketsComponent implements OnInit {
  assignedTicketsdata: any[] = [];
  isLoading = true;
  isError = false;
  errorMessage: string = '';

  // Define the state enum mapping
  stateEnum: { [key: number]: string } = {
    1: 'OPEN',
    2: 'INPROGRESS',
    3: 'WAITING',
    4: 'CLOSED',
  };

  constructor(
    private http: HttpClient,
    private cookieService: CookieService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchAssignedTickets();
  }

  // Fetch assigned tickets from the API
  fetchAssignedTickets() {
    const token = this.cookieService.get('accessToken');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });
  
    this.http.get('https://localhost:7292/ticket/assigned', { headers }).subscribe({
      next: (response: any) => {
        // console.log('Assigned Tickets API Response:', response); // Log the response to inspect it
        // alert('Assigned Tickets Fetched: ' + JSON.stringify(response)); // Show the fetched response in an alert
        this.assignedTicketsdata = response; // Assign the response to the correct property
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching tickets', err);
        this.isError = true;
        this.errorMessage = 'Error fetching tickets. Please try again later.';
        this.isLoading = false;
      },
    });
  }
  

  // Get the string representation of the state based on the enum
  getStateString(state: number): string {
    return this.stateEnum[state] || 'Unknown';
  }

  // Navigate to the edit ticket page with the selected ticket's ID
  editTicket(ticketId: number) {
    this.router.navigate([`/ticket/edit/${ticketId}`]);
  }
}
