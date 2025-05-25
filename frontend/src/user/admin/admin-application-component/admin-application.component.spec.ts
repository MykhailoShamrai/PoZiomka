import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminApplicationComponentComponent } from './admin-application-component.component';

describe('AdminApplicationComponentComponent', () => {
  let component: AdminApplicationComponentComponent;
  let fixture: ComponentFixture<AdminApplicationComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminApplicationComponentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminApplicationComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
