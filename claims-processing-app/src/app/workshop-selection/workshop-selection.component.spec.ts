import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkshopSelectionComponent } from './workshop-selection.component';

describe('WorkshopSelectionComponent', () => {
  let component: WorkshopSelectionComponent;
  let fixture: ComponentFixture<WorkshopSelectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkshopSelectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkshopSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
