import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { HttpClient, HttpHeaders, HttpClientModule } from '@angular/common/http';  // Import HttpClientModule if in standalone
import { CookieService } from 'ngx-cookie-service';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { jwtDecode } from "jwt-decode";

@Injectable({
  providedIn: 'root',
})
export class AdminGuard implements CanActivate {
  constructor(
    private http: HttpClient,
    private router: Router,
    private cookieService: CookieService
  ) {}

  canActivate(): Observable<boolean> {
    const token = this.cookieService.get('accessToken');

    if (!token) {
      this.router.navigate(['/login']);
      return of(false);
    }

    try {
      const decodedToken: any = jwtDecode(token);
      const userId = decodedToken.sub;

      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      });

      return this.http.get(`https://localhost:7292/user/${userId}`, { headers }).pipe(
        map((response: any) => {
          if (response && response.role === 2) {  // Admin role
            return true;
          } else {
            this.router.navigate(['/not-authorized']);
            return false;
          }
        }),
        catchError(() => {
          this.router.navigate(['/login']);
          return of(false);
        })
      );
    } catch (error) {
      this.router.navigate(['/login']);
      return of(false);
    }
  }
}
