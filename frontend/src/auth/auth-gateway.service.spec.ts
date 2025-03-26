import { TestBed } from '@angular/core/testing';

import { AuthGatewayService } from './auth-gateway.service';

describe('AuthGatewayService', () => {
  let service: AuthGatewayService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthGatewayService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
