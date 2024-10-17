import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select'; // Import MatSelectModule for dropdown
import { MatCheckboxModule } from '@angular/material/checkbox'; // Import MatCheckboxModule for checkbox
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-edit-user',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule, // Add MatSelectModule here
    MatCheckboxModule, // Add MatCheckboxModule for the checkbox
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
      role: ['', [Validators.required]],
      blocked: [false]
    });

    this.userId = Number(this.route.snapshot.paramMap.get('userId'));
  }

  ngOnInit(): void {
    this.fetchUserDetails();
  }

  fetchUserDetails() {
    const token = this.cookieService.get('accessToken');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    this.http.get(`https://localhost:7292/user/${this.userId}`, { headers }).subscribe({
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

  onSubmit() {
    if (this.editUserForm.valid) {
      const token = this.cookieService.get('accessToken');
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      const updatedUserData = this.editUserForm.value;

      this.http.put(`https://localhost:7292/user/${this.userId}`, updatedUserData, { headers }).subscribe({
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
