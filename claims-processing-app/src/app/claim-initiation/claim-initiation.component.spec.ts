import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClaimInitiationComponent } from './claim-initiation.component';

describe('ClaimInitiationComponent', () => {
  let component: ClaimInitiationComponent;
  let fixture: ComponentFixture<ClaimInitiationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClaimInitiationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClaimInitiationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
