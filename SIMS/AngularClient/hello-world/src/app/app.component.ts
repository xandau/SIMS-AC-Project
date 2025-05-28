import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar'; // Import MatToolbarModule
import { RouterModule, Router, ActivatedRoute, NavigationEnd } from '@angular/router'; // Import Router and ActivatedRoute
import { CommonModule } from '@angular/common'; // Import CommonModule for *ngIf
import { filter } from 'rxjs/operators'; // Import filter
import { CookieService } from 'ngx-cookie-service'; // Import CookieService for token management
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http'; // Import HttpClient and HttpClientModule for HTTP requests

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    MatToolbarModule,  // Add MatToolbarModule
    RouterModule,      // Add RouterModule to support routing
    CommonModule,      // Add CommonModule for *ngIf directive
    HttpClientModule   // Add HttpClientModule to support HTTP requests
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'basic-website';
  showCreateTicketButton = false;  // Variable to control the visibility of the Create Ticket button
  isAuthenticated = false;  // Declare and initialize isAuthenticated

  apiUrl = (window as any).__env?.WEB_API_URL;
    ownURL = (window as any).__env?.WEB_OWN_DOMAIN; 
  ownDomain = '';

  constructor(private router: Router, private route: ActivatedRoute, private cookieService: CookieService, private http: HttpClient) {
    // Listen for route changes and check if the current route is dashboard or tickets
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        const currentUrl = event.urlAfterRedirects;
        this.showCreateTicketButton = currentUrl.includes('/dashboard') || currentUrl.includes('/tickets') || currentUrl.includes('/assigned-tickets') || currentUrl.includes('/created-tickets');
        this.checkAuthentication();  // Check if the user is authenticated
      });
  }

    if (this.ownURL) {
      try {
        const url = new URL(this.ownURL);
        this.ownDomain = url.hostname;
      } catch (e) {
        console.error('Error parsing ownURL for domain extraction in AppComponent:', e);
        this.ownDomain = ''; // Or a default/fallback domain like 'localhost' if appropriate
      }
    }

    
  checkAuthentication() {
    // Check for the presence of an accessToken in cookies
    const token = this.cookieService.get('accessToken');
    this.isAuthenticated = !!token;  // Set isAuthenticated to true if token exists
  }

  logout() {
    // Clear authentication cookies
    this.cookieService.delete('accessToken', '/', this.ownDomain);
    this.cookieService.delete('refreshToken', '/', this.ownDomain);
    this.isAuthenticated = false;  // Update authentication status
    this.router.navigate(['/home']);  // Redirect to home after logout
  }

  openCreateTicketForm() {
    this.router.navigate(['/create-ticket']);  // Navigate to the create ticket page
  }

  refreshToken() {
    const refreshToken = this.cookieService.get('refreshToken');
    
    if (refreshToken) {
      const headers = new HttpHeaders({
        'Content-Type': 'application/json'
      });
  
      // Make the request to refresh the token, sending the token field as "token"
      this.http.post(`${this.apiUrl}/auth/refresh`, { token: refreshToken }, { headers })
        .subscribe({
          next: (response: any) => {
            // Assuming response contains the new tokens
            this.cookieService.set('accessToken', response.accessToken, 1, '/', 'localhost', false, 'Lax');
            this.cookieService.set('refreshToken', response.refreshToken, 30, '/', 'localhost', false, 'Lax');
            console.log('Token refreshed successfully');
  
            // Show alert for success
            alert('Token refreshed successfully');
          },
          error: (err) => {
            console.error('Error refreshing token', err);
            // Optionally redirect to login page if the refresh fails
            this.router.navigate(['/login']);
          }
        });
    } else {
      console.error('No refresh token found');
      this.router.navigate(['/login']); // Redirect to login if no refresh token
    }
  }  
}
