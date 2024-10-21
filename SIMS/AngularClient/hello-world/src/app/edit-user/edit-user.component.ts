import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-edit-user',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatCheckboxModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.css'],
})
export class EditUserComponent implements OnInit {
  editUserForm: FormGroup;
  userId: number;
  isLoading = true;
  isError = false;
  errorMessage: string = '';

  // Enum-like mapping for user roles
  roles = [
    { value: 1, label: 'USER' },
    { value: 2, label: 'ADMIN' }
  ];

  apiUrl = (window as any).__env?.WEB_API_URL;
  
  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private cookieService: CookieService
  ) {
    this.editUserForm = this.fb.group({
      userName: ['', [Validators.required]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      role: ['', [Validators.required]],  // Role field uses the enum mapping
    });

    this.userId = Number(this.route.snapshot.paramMap.get('userId'));
  }

  ngOnInit(): void {
    this.fetchUserDetails();
  }

  // Fetch the current user details based on userId
  fetchUserDetails() {
    const token = this.cookieService.get('accessToken');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    this.http.get(`${this.apiUrl}/user/${this.userId}`, { headers }).subscribe({
      next: (response: any) => {
        this.editUserForm.patchValue(response);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching user details', err);
        this.isError = true;
        this.errorMessage = 'Error fetching user details. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  // Handle form submission to update user details
  onSubmit() {
    if (this.editUserForm.valid) {
      const token = this.cookieService.get('accessToken');
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      const updatedUserData = {
        ...this.editUserForm.value,  // Spread the form values (userName, firstName, lastName, etc.)
        id: this.userId          // Add userId to the payload explicitly
      };

      // Send the form data with the correct role value and userId
      this.http.put(`${this.apiUrl}/user/${this.userId}`, updatedUserData, { headers }).subscribe({
        next: (response: any) => {
          console.log('User updated successfully!', response);
          this.router.navigate(['/admin-dashboard']);  // Navigate back to admin dashboard after successful update
        },
        error: (err) => {
          console.error('Error updating user', err);
          this.isError = true;
          this.errorMessage = 'Error updating user. Please try again later.';
        }
      });
    }
  }
}
