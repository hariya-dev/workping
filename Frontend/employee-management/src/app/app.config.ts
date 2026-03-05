// app.config.ts
// Cấu hình chính của ứng dụng Angular

import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    // Zone change detection với event coalescing để tối ưu performance
    provideZoneChangeDetection({ eventCoalescing: true }),
    
    // Router với các routes đã định nghĩa
    provideRouter(routes),
    
    // HTTP Client với auth interceptor
    provideHttpClient(
      withInterceptors([authInterceptor])
    )
  ]
};
