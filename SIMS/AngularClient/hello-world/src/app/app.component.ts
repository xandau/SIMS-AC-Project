import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar'; // Import MatToolbarModule
import { RouterModule, Router, ActivatedRoute, NavigationEnd } from '@angular/router'; // Import Router and ActivatedRoute
import { CommonModule } from '@angular/common'; // Import CommonModule for *ngIf
import { filter } from 'rxjs/operators'; // Import filter
import { CookieService } from 'ngx-cookie-service'; // Import CookieService for token management

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    MatToolbarModule,  // Add MatToolbarModule
    RouterModule,      // Add RouterModule to support routing
    CommonModule       // Add CommonModule for *ngIf directive
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'basic-website';
  showCreateTicketButton = false;  // Variable to control the visibility of the Create Ticket button
  isAuthenticated = false;  // Declare and initialize isAuthenticated

  constructor(private router: Router, private route: ActivatedRoute, private cookieService: CookieService) {
    // Listen for route changes and check if the current route is dashboard or tickets
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        const currentUrl = event.urlAfterRedirects;
        this.showCreateTicketButton = currentUrl.includes('/dashboard') || currentUrl.includes('/tickets');
        this.checkAuthentication();  // Check if the user is authenticated
      });
  }

  checkAuthentication() {
    // Check for the presence of an accessToken in cookies
    const token = this.cookieService.get('accessToken');
    this.isAuthenticated = !!token;  // Set isAuthenticated to true if token exists
  }

  openCreateTicketForm() {
    this.router.navigate(['/create-ticket']);  // Navigate to the create ticket page
  }
}
