import { Component, computed, input, output } from '@angular/core';
import { ButtonComponent } from '../../general-components/button/button.component';

@Component({
  selector: 'app-expense-line',
  imports: [ButtonComponent],
  templateUrl: './expense-line.component.html',
  styleUrl: './expense-line.component.css',
})
export class ExpenseLineComponent {
  description = input.required<string>();
  amount = input.required<number>();
  date = input.required<Date>();

  localizedAmount = computed(() => {
    return this.amount().toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    });
  });

  localizedDate = computed(() => {
    let date = this.date();
    let month = '';
    if (date.getMonth() < 10) {
      month = (date.getMonth() + 1).toString().padStart(2, '0');
    } else {
      month = (date.getMonth() + 1).toString();
    }

    return [
      `${date.getDate().toString().padStart(2, '0')}/${month}/${date
        .getFullYear()
        .toString()}`,
    ];
  });

  deleteClicked = output<void>();
  editClicked = output<void>();
}
