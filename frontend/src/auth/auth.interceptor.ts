import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthGatewayService } from './auth-gateway.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthGatewayService);
  const token = authService.getToken();
  
  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(authReq);
  }
  
  return next(req);
};