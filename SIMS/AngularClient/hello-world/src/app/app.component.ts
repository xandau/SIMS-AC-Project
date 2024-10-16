import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar'; // Import MatToolbarModule
import { RouterModule, Router, ActivatedRoute, NavigationEnd } from '@angular/router'; // Import Router and ActivatedRoute
import { CommonModule } from '@angular/common'; // Import CommonModule for *ngIf
import { filter } from 'rxjs/operators'; // Import filter

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

  constructor(private router: Router, private route: ActivatedRoute) {
    // Listen for route changes and check if the current route is dashboard or tickets
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        const currentUrl = event.urlAfterRedirects;
        this.showCreateTicketButton = currentUrl.includes('/dashboard') || currentUrl.includes('/tickets');
      });
  }

  openCreateTicketForm() {
    this.router.navigate(['/create-ticket']);  // Navigate to the create ticket page
  }
}
