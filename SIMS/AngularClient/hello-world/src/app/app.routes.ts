import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TicketComponent } from './ticket/ticket.component';
import { AuthGuard } from './auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },  // Make Home route accessible to everyone
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent, 
    canActivate: [AuthGuard]  // Protected route
  },
  { 
    path: 'tickets', 
    component: TicketComponent, 
    canActivate: [AuthGuard]  // Protect tickets with AuthGuard
  },
  { path: '**', redirectTo: '' }  // Redirect any unknown routes to the home page
];
