import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserApplicationComponent } from './user-application.component';

describe('UserApplicationComponent', () => {
  let component: UserApplicationComponent;
  let fixture: ComponentFixture<UserApplicationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserApplicationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserApplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
