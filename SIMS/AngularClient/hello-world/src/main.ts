import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideHttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { CookieService } from 'ngx-cookie-service';
import { AuthInterceptor } from './app/auth.interceptor';
import { AuthGuard } from './app/auth.guard'; 
import { routes } from './app/app.routes';

// Bootstrap the application
bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(),
    provideRouter(routes),
    provideAnimations(),
    CookieService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ]
}).then(() => {
  // Now check if the environment variables are loaded via config.js
  if ((window as any).__env) {
    console.log('Environment variables loaded:', (window as any).__env);
  } else {
    console.warn('No environment variables found.');
  }
});
