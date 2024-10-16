import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';  // Import MatFormFieldModule
import { MatInputModule } from '@angular/material/input';            // Import MatInputModule
import { MatButtonModule } from '@angular/material/button';          // Import MatButtonModule
import { MatCardModule } from '@angular/material/card';              // Import MatCardModule
import { CommonModule } from '@angular/common';                      // Import CommonModule for *ngIf and other directives

@Component({
  selector: 'app-ticket-create',
  standalone: true,
  imports: [
    MatFormFieldModule,  // Add Material modules for form fields
    MatInputModule,      // Add Material input module for form inputs
    MatButtonModule,     // Add Material button module
    MatCardModule,       // Add Material card module for mat-card
    ReactiveFormsModule, // Add ReactiveFormsModule for using formGroup
    CommonModule         // Import CommonModule for structural directives like *ngIf
  ],
  templateUrl: './ticket-create.component.html',
  styleUrls: ['./ticket-create.component.css'],
})
export class TicketCreateComponent {
  ticketForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {
    this.ticketForm = this.fb.group({
      TITLE: ['', [Validators.required]],
      DESCRIPTION: ['', [Validators.required]],
      STATE: [2],  // Default state for new tickets
      CREATORID: [1],  // Assuming current user ID is 1
      Severity: [5],
      CVE: [''],
    });
  }

  onSubmit() {
    if (this.ticketForm.valid) {
      const ticketData = this.ticketForm.value;
      this.http.post('/api/tickets', ticketData).subscribe({
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
