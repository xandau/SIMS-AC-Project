import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  template: `
    <div class="home-container">
      <h1>Welcome to the Home Page</h1>
    </div>
  `,
  styles: [`
    .home-container {
      padding: 20px;
    }
  `]
})
export class HomeComponent {}
