import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TicketComponent } from './ticket/ticket.component';
import { TicketCreateComponent } from './ticket-create/ticket-create.component'; 
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { EditUserComponent } from './edit-user/edit-user.component';
import { AuthGuard } from './auth.guard'; 

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
    
    component: TicketCreateComponent, 
    canActivate: [AuthGuard]  
  },
  {
    path: 'admin-dashboard', 
    component: AdminDashboardComponent, 
    canActivate: [AuthGuard]  
  },
  {
    path: 'user/edit/:userId', 
    component: EditUserComponent, 
    canActivate: [AuthGuard]  
  },
  { path: '**', redirectTo: '' }
];
