import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TicketComponent } from './ticket/ticket.component';
import { TicketCreateComponent } from './ticket-create/ticket-create.component'; 
import { CreatedTicketsComponent } from './created-tickets/created-tickets.component'; 
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { EditUserComponent } from './edit-user/edit-user.component';
import { EditTicketComponent } from './edit-ticket/edit-ticket.component';
import { AssignedTicketsComponent } from './assigned-tickets/assigned-tickets.component';
import { UserGuard } from './user.guard'; 
import { AdminGuard } from './admin.guard'; 

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent, 
    canActivate: [UserGuard] 
  },
  { 
    path: 'tickets', 
    component: TicketComponent, 
    canActivate: [UserGuard] 
  },
  { 
    path: 'create-ticket', 
    
    component: TicketCreateComponent, 
    canActivate: [UserGuard]  
  },
  {
    path: 'admin-dashboard', 
    component: AdminDashboardComponent, 
    canActivate: [AdminGuard]  
  },
  {
    path: 'user/edit/:userId', 
    component: EditUserComponent, 
    canActivate: [AdminGuard]  
  },
  {
    path: 'created-tickets',  
    component: CreatedTicketsComponent,
    canActivate: [UserGuard],
  },
  {
    path: 'ticket/edit/:ticketId',  
    component: EditTicketComponent,
    canActivate: [UserGuard],
  },
  {
    path: 'assigned-tickets',  
    component: AssignedTicketsComponent,
    canActivate: [UserGuard],
  },
  { path: '**', redirectTo: '' }
];
