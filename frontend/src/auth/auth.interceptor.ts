import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthGatewayService } from './auth-gateway.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authReq = req.clone({
    withCredentials: true
  });
  return next(authReq);
};