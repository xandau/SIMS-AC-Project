import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';  
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service'; 

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
    HttpClientModule,  
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;
  isError: boolean = false;
  errorMessage: string = '';

  apiUrl = (window as any).__env?.WEB_API_URL;
  ownURL = (window as any).__env?.WEB_OWN_DOMAIN;
  apiDomain = ''; // To store the extracted domain

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,    
    private router: Router,
    private cookieService: CookieService  
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });

    // Extract domain from apiUrl
    if (this.apiUrl) {
      try {
        const url = new URL(this.apiUrl);
        this.apiDomain = url.hostname;
      } catch (e) {
        console.error('Error parsing API URL for domain extraction:', e);
        // Fallback or handle error if apiUrl is not a valid URL
        // For example, if running locally and apiUrl might not have a scheme
        // or if you want a default domain for local testing.
        // However, for deployed environments, apiUrl should be a full valid URL.
        // If it's just a hostname, new URL() will fail.
        // A more robust regex might be needed if apiUrl isn't always a full URL.
        // For now, assuming apiUrl is like "http://domain.com" or "https://domain.com"
      }
    }
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;

      // console.log('API URL:', this.apiUrl); 
      // console.log('API Domain for cookie:', this.apiDomain);

      this.http.post(`${this.apiUrl}/auth/login`, loginData).subscribe({
        next: (response: any) => {
          console.log('Login successful!', response);

          // Determine domain for cookie
          // If apiDomain is successfully extracted, use it. Otherwise, consider a fallback or error.
          // For security, it's best if apiDomain is correctly set.
          // Setting a cookie for a domain that doesn't match the API won't work.
          const cookieDomain = this.ownURL || undefined; // Use undefined if domain couldn't be parsed, letting browser decide based on current doc.
                                                        // Or, ensure apiDomain is always valid.
                                                        // If apiDomain is an empty string, some browsers might set it for the current host.
                                                        // For cross-origin, it MUST be the API's domain.

          // if (!cookieDomain) {
          if(!this.apiDomain) {
            console.warn('Cookie domain could not be determined from API URL. Cookies might not be set correctly for cross-origin requests.');
            // Decide on a fallback if necessary, e.g. for local development if apiUrl is just 'localhost'
            // However, for your ALB setup, apiUrl should be the full URL.
          }
          
          // Store tokens in cookies, now using the extracted domain
          this.cookieService.set('accessToken', response.accessToken, { expires: 1, path: '/', domain: cookieDomain, secure: false, sameSite: 'Lax' });
          this.cookieService.set('refreshToken', response.refreshToken, { expires: 30, path: '/', domain: cookieDomain, secure: false, sameSite: 'Lax' });

          // Redirect to home page after login
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          console.error('Login failed', err);
          this.isError = true;
          this.errorMessage = 'Login failed. Please check your credentials.';
        }
      });
    }
  }
}
