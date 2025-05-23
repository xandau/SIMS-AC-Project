import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-ticket-create',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatSelectModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './ticket-create.component.html',
  styleUrls: ['./ticket-create.component.css'],
})
export class TicketCreateComponent {
  ticketForm: FormGroup;
  isSuccess: boolean = false;  // Flag to indicate if the ticket is successfully created
  isError: boolean = false;    // Flag to indicate if there was an error
  errorMessage: string = '';   // Store the error message to display
  countdown: number = 5;       // Countdown in seconds before redirecting to dashboard

  apiUrl = (window as any).__env?.WEB_API_URL;
  
  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.ticketForm = this.fb.group({
      TITLE: ['', [Validators.required]],                   // Required
      DESCRIPTION: ['', [Validators.required]],             // Required
      STATE: [2, [Validators.required]],                    // Required, default to 2
      Severity: [5, [Validators.required]],                 // Required
      CVE: ['', [Validators.required]],                     // Required
      assignedPersonID: ['', [Validators.required]],        // New required field
      referenceID: [''],                                   // Optional
    });
  }

  onSubmit() {
    if (this.ticketForm.valid) {
      // Retrieve the JWT token from the cookies
      const token = this.cookieService.get('accessToken');

      // Decode the JWT to extract the user ID (sub)
      const decodedToken: any = jwtDecode(token);
      const userId = decodedToken?.sub;

      // Prepare the ticket data
      const ticketData = {
        TITLE: this.ticketForm.get('TITLE')?.value,
        DESCRIPTION: this.ticketForm.get('DESCRIPTION')?.value,
        STATE: parseInt(this.ticketForm.get('STATE')?.value, 10),  // Ensure STATE is sent as a number
        CREATORID: userId,  // Use the decoded user ID (sub)
        Severity: this.ticketForm.get('Severity')?.value,
        CVE: this.ticketForm.get('CVE')?.value,
        assignedPersonID: this.ticketForm.get('assignedPersonID')?.value, // New field
        referenceID: this.ticketForm.get('referenceID')?.value, // Optional field
      };

      // Set the headers with the Bearer token
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      // Make the HTTP POST request with the Bearer token in the headers and JSON payload
      this.http.post(`${this.apiUrl}/ticket`, JSON.stringify(ticketData), { headers }).subscribe({
        next: (response: any) => {
          console.log('Ticket created successfully!', response);
          this.isSuccess = true;  // Mark as successful creation
          this.isError = false;   // Reset error flag

          // Start the countdown timer
          const countdownInterval = setInterval(() => {
            this.countdown--;  // Decrement the countdown by 1 every second
            if (this.countdown === 0) {
              clearInterval(countdownInterval);  // Stop the countdown
              this.router.navigate(['/dashboard']);  // Redirect to dashboard
            }
          }, 1000);  // 1000 ms = 1 second
        },
        error: (err) => {
          console.error('Error creating ticket', err);
          this.isError = true;  // Set error flag to true
          this.errorMessage = 'There was an error creating the ticket. Please try again.'; // Set a custom error message
        }
      });
    }
  }
}