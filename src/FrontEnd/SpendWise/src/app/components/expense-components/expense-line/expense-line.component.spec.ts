import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseLineComponent } from './expense-line.component';

describe('ExpenseLineComponent', () => {
  let component: ExpenseLineComponent;
  let fixture: ComponentFixture<ExpenseLineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExpenseLineComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpenseLineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
