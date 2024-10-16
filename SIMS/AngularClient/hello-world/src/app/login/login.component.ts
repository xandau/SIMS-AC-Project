import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';  // Ensure HttpClientModule is imported
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service'; // Import CookieService

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    HttpClientModule,  // Add HttpClientModule here
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,    // Ensure HttpClient is injected
    private router: Router,
    private cookieService: CookieService  // Inject CookieService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;

      this.http.post('https://localhost:7292/auth/login', loginData).subscribe({
        next: (response: any) => {
          console.log('Login successful!', response);

          // Store tokens in cookies
          this.setCookies(response.accessToken, response.refreshToken);

          // Read and log the stored cookies
          this.logCurrentCookies();

          // Redirect to home page after login
          // this.router.navigate(['/']);
        },
        error: (err) => {
          console.error('Login failed', err);
        }
      });
    }
  }

  // Set the access and refresh tokens in cookies
  setCookies(accessToken: string, refreshToken: string) {
    try {
      // Store access token (1 day expiry)
      this.cookieService.set('accessToken', accessToken, 1, '/', 'localhost', false, 'Lax');
      // Store refresh token (30 days expiry)
      this.cookieService.set('refreshToken', refreshToken, 30, '/', 'localhost', false, 'Lax');
    } catch (error) {
      console.error('Error setting cookies:', error);
    }
  }

  // Function to log current cookies
  logCurrentCookies() {
    const accessToken = this.cookieService.get('accessToken');
    const refreshToken = this.cookieService.get('refreshToken');
    
    console.log('Access Token:', accessToken);
    console.log('Refresh Token:', refreshToken);
  }
}
