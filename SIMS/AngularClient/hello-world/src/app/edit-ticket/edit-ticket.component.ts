import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms'; // Import ReactiveFormsModule
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-ticket',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    CommonModule,
    MatCardModule,
    ReactiveFormsModule  // Ensure ReactiveFormsModule is imported here
  ],
  templateUrl: './edit-ticket.component.html',
  styleUrls: ['./edit-ticket.component.css']
})
export class EditTicketComponent implements OnInit {
  apiUrl = (window as any).__env?.WEB_API_URL;

  ticketForm: FormGroup;
  ticketId: number;
  isLoading = true;
  isError = false;
  errorMessage: string = '';
  userId: number;
  creatorID: number = 0;  // Store the creator ID

  // Enum for ticket states
  stateEnum = [
    { value: 1, label: 'OPEN' },
    { value: 2, label: 'INPROGRESS' },
    { value: 3, label: 'WAITING' },
    { value: 4, label: 'CLOSED' }
  ];


  

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.ticketForm = this.fb.group({
      title: ['', [Validators.required]],
      description: ['', [Validators.required]],
      state: ['', [Validators.required]],
      severity: [0, [Validators.required, Validators.min(1), Validators.max(10)]],
      cve: [''],
      assignedPersonID: [null]
    });

    this.ticketId = Number(this.route.snapshot.paramMap.get('ticketId'));

    // Decode the JWT token to get the user ID
    const token = this.cookieService.get('accessToken');
    const decodedToken: any = jwtDecode(token);
    this.userId = parseInt(decodedToken.sub, 10);  // Extract the user ID from the token
  }

  ngOnInit(): void {
    console.log('API URL:', this.apiUrl);
    this.fetchTicketDetails();
  }

  // Fetch ticket details for editing
  fetchTicketDetails() {

    const token = this.cookieService.get('accessToken');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    this.http.get(`${this.apiUrl}/ticket/${this.ticketId}`, { headers }).subscribe({
      next: (response: any) => {
        // Check if the current user is the creator or assigned person
        if (response.creatorID === this.userId || response.assignedPersonID === this.userId) {
          this.ticketForm.patchValue(response);
          this.creatorID = response.creatorID;  // Store the creatorID for later use
          this.isLoading = false;
        } else {
          // If the user is not allowed to access the ticket, redirect or show an error
          this.isError = true;
          this.errorMessage = 'You are not authorized to edit this ticket.';
          this.router.navigate(['/tickets']);
        }
      },
      error: (err) => {
        console.error('Error fetching ticket details', err);
        this.isError = true;
        this.errorMessage = 'Error fetching ticket details. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  // Submit edited ticket
  onSubmit() {
    if (this.ticketForm.valid) {
      const token = this.cookieService.get('accessToken');
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      // Include the ticketId as 'id' and the 'creatorID' in the payload
      const updatedTicketData = {
        ...this.ticketForm.value,  // Spread the form values (title, description, etc.)
        id: this.ticketId,         // Send ticketId as id
        creatorID: this.creatorID  // Include the original creatorID
      };

      this.http.put(`${this.apiUrl}/ticket/${this.ticketId}`, updatedTicketData, { headers }).subscribe({
        next: (response: any) => {
          console.log('Ticket updated successfully!', response);
          this.router.navigate(['/created-tickets']);  // Redirect back to created tickets page after update
        },
        error: (err) => {
          console.error('Error updating ticket', err);
          this.isError = true;
          this.errorMessage = 'Error updating ticket. Please try again later.';
        }
      });
    }
  }
}
