// src/app/app.config.ts
import {
  ApplicationConfig,
  provideZoneChangeDetection,
  importProvidersFrom
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';

import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import {
  HTTP_INTERCEPTORS,
  provideHttpClient,
  withInterceptorsFromDi
} from '@angular/common/http';

import { CookieService } from 'ngx-cookie-service';   // <-- use service, not module
import { AuthInterceptor } from './core/auth/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    /* core Angular providers */
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(),
    provideAnimationsAsync(),

    /* cookie + interceptor */
    CookieService,                                         // <-- simple provider
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },

    /* HttpClient with all interceptors from DI */
    provideHttpClient(withInterceptorsFromDi())
  ]
};
