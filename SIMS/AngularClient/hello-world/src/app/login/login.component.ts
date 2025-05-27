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

    console.log('LoginComponent constructor: Initial apiUrl:', this.apiUrl); // Log initial apiUrl
    // Extract domain from apiUrl
    if (this.apiUrl) {
      try {
        const url = new URL(this.apiUrl);
        this.apiDomain = url.hostname;
        console.log('LoginComponent constructor: Extracted apiDomain:', this.apiDomain); // Log extracted domain
      } catch (e) {
        console.error('LoginComponent constructor: Error parsing API URL for domain extraction:', e);
        console.error('LoginComponent constructor: apiUrl that caused error:', this.apiUrl);
        this.apiDomain = ''; // Ensure it's reset if parsing fails
      }
    } else {
      console.warn('LoginComponent constructor: apiUrl is not set.');
    }
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;

      console.log('onSubmit: Current apiUrl:', this.apiUrl);
      console.log('onSubmit: Current apiDomain for cookie:', this.apiDomain);

      this.http.post(`${this.apiUrl}/auth/login`, loginData).subscribe({
        next: (response: any) => {
          console.log('Login successful! Full response:', response); // Log the full response

          if (!response || !response.accessToken || !response.refreshToken) {
            console.error('Login response missing accessToken or refreshToken:', response);
            this.isError = true;
            this.errorMessage = 'Login response from server was incomplete.';
            return; // Stop if tokens are missing
          }

          const cookieDomain = this.apiDomain || undefined;
          console.log('Setting cookies with domain:', cookieDomain);
          console.log('Access Token from response:', response.accessToken);
          console.log('Refresh Token from response:', response.refreshToken);

          // Store tokens in cookies, now using the extracted domain
          this.cookieService.set('accessToken', response.accessToken, { expires: 1, path: '/', domain: cookieDomain, secure: false, sameSite: 'Lax' });
          this.cookieService.set('refreshToken', response.refreshToken, { expires: 30, path: '/', domain: cookieDomain, secure: false, sameSite: 'Lax' });

          // Verify if cookies are set (this is a synchronous check, browser might take a moment)
          console.log('Cookie "accessToken" after set:', this.cookieService.get('accessToken'));
          console.log('Cookie "refreshToken" after set:', this.cookieService.get('refreshToken'));

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
