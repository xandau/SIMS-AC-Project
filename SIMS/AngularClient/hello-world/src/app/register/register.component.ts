import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registerForm: FormGroup;
  isError = false; // Flag to track if there is an error
  errorMessage = ''; // Message to show specific error details

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      userName: ['', [Validators.required]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit() {
    if (this.registerForm.valid) {
      const registerData = this.registerForm.value;

      this.http.post('https://localhost:7292/api/register', registerData).subscribe({
        next: (response: any) => {
          console.log('Registration successful!', response);
          this.router.navigate(['/login']); // Navigate to login after successful registration
        },
        error: (err: HttpErrorResponse) => {
          console.error('Login failed', err);
          this.isError = true;
          if (err.status === 0) {
            // Network error (e.g., backend server is down)
            this.errorMessage = 'Network error: Unable to connect to the server.';
          } else if (err.status === 401) {
            // Unauthorized error (incorrect credentials)
            this.errorMessage = 'Invalid email or password. Please try again.';
          } else {
            // Other errors
            this.errorMessage = 'An unexpected error occurred. Please try again later.';
          }
        },
      });
    } else {
      this.isError = true;
      this.errorMessage = 'Please fill in all required fields correctly.';
    }
  }
}
