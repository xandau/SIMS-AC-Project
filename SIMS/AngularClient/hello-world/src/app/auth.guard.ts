import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private cookieService: CookieService, private router: Router) {}

  canActivate(): boolean {
    const accessToken = this.cookieService.get('accessToken');
    
    if (accessToken) {
      // If JWT token exists in cookies, allow access to the route
      return true;
    } else {
      // If no token, redirect to login
      this.router.navigate(['/login']);
      return false;
    }
  }
}
