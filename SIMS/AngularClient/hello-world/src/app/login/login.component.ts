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
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;

      // console.log('API URL:', this.apiUrl); 

      this.http.post(`${this.apiUrl}/auth/login`, loginData).subscribe({
        next: (response: any) => {
          console.log('Login successful!', response);

          // Store tokens in cookies
          this.cookieService.set('accessToken', response.accessToken, 1, '/', 'localhost', false, 'Lax');
          this.cookieService.set('refreshToken', response.refreshToken, 30, '/', 'localhost', false, 'Lax');

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
