import { Component, output } from '@angular/core';
import { ExpensesService } from '../../../services/expenses.service';
import { ButtonComponent } from '../../general-components/button/button.component';

@Component({
  selector: 'app-action-bar-component',
  imports: [ButtonComponent],
  templateUrl: './action-bar.component.html',
  styleUrl: './action-bar.component.css',
  providers: [ExpensesService],
})
export class ActionBarComponent {
  singleExpenseModalOpen = output<void>();
}
