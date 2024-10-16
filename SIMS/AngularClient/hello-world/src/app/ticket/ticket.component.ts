import { Component } from '@angular/core';

@Component({
  selector: 'app-ticket',
  standalone: true,
  template: `
    <div class="ticket-container">
      <h2>Tickets</h2>
      <p>This is where your ticket information will be displayed.</p>
    </div>
  `,
  styles: [`
    .ticket-container {
      padding: 20px;
    }
  `]
})
export class TicketComponent {}
