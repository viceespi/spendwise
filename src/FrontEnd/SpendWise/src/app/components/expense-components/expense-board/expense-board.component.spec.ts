import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseBoardComponent } from './expense-board.component';

describe('ExpenseBoardComponent', () => {
  let component: ExpenseBoardComponent;
  let fixture: ComponentFixture<ExpenseBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExpenseBoardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpenseBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
