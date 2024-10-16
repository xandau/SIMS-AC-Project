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

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.ticketForm = this.fb.group({
      TITLE: ['', [Validators.required]],
      DESCRIPTION: ['', [Validators.required]],
      STATE: [2, [Validators.required]],  // Default state for new tickets, required
      CREATORID: [1],  // Assuming current user ID is 1
      Severity: [5],
      CVE: [''],
    });
  }

  onSubmit() {
    if (this.ticketForm.valid) {
      // Prepare the ticket data
      const ticketData = {
        TITLE: this.ticketForm.get('TITLE')?.value,
        DESCRIPTION: this.ticketForm.get('DESCRIPTION')?.value,
        STATE: parseInt(this.ticketForm.get('STATE')?.value, 10),  // Ensure STATE is sent as a number
        CREATORID: this.ticketForm.get('CREATORID')?.value,
        Severity: this.ticketForm.get('Severity')?.value,
        CVE: this.ticketForm.get('CVE')?.value
      };

      // Retrieve the JWT token from the cookies
      const token = this.cookieService.get('accessToken');

      // Set the headers with the Bearer token
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      // Convert the ticketData object to JSON using JSON.stringify
      const jsonTicketData = JSON.stringify(ticketData);

      // Make the HTTP POST request with the Bearer token in the headers and JSON payload
      this.http.post('https://localhost:7292/ticket', jsonTicketData, { headers }).subscribe({
        next: (response: any) => {
          console.log('Ticket created successfully!', response);
          this.router.navigate(['/tickets']);  // Redirect to ticket list after creation
        },
        error: (err) => {
          console.error('Error creating ticket', err);
        }
      });
    }
  }
}
