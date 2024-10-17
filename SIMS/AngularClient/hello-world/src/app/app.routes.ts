import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TicketComponent } from './ticket/ticket.component';
import { TicketCreateComponent } from './ticket-create/ticket-create.component'; // Import the correct component
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AuthGuard } from './auth.guard';  // Import AuthGuard if necessary

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'tickets', 
    component: TicketComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'create-ticket', 
    
    component: TicketCreateComponent, // Reference the correct component name
    canActivate: [AuthGuard]  // Protect if necessary
  },
  {
    path: 'admin-dashboard', 
    component: AdminDashboardComponent, // Reference the correct component name
    canActivate: [AuthGuard]  // Protect if necessary
  },
  { path: '**', redirectTo: '' }
];
