import { Component } from '@angular/core';
import { ActionBarComponent } from '../../components/expense-components/action-bar/action-bar.component';
import { ExpenseLineComponent } from '../../components/expense-components/expense-line/expense-line.component';

@Component({
  selector: 'app-expenses',
  imports: [ActionBarComponent, ExpenseLineComponent],
  templateUrl: './expenses.component.html',
  styleUrl: './expenses.component.css'
})
export class ExpensesComponent {

}
