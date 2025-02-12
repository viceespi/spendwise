import { Component, ElementRef, ViewChild } from "@angular/core";
import { ExpensesService } from "../../../services/expenses.service";
import { ButtonComponent } from "../../general-components/button/button.component";
import { ModalComponent } from "../../general-components/modal/modal.component";
import { NewExpenseDTO } from "../../../models/newExpenseDTO";


@Component({
  selector: 'app-action-bar-component',
  imports: [ButtonComponent, ModalComponent],
  templateUrl: './action-bar.component.html',
  styleUrl: './action-bar.component.css',
  providers: [ExpensesService]
})

export class ActionBarComponent {
  constructor(private expensesService: ExpensesService) {}

  @ViewChild('description') descriptionElement!: ElementRef;
  @ViewChild('amount') amountElement!: ElementRef;
  @ViewChild('date') dateElement!: ElementRef;
  @ViewChild('modal') modalElement!: ElementRef;

  getAmount(): number{
    let amount = Number(this.amountElement.nativeElement.value);
    return amount;
  }

  getDate(): Date {
    let date = new Date(this.dateElement.nativeElement.value);
    return date;
  }

  createExpenseCall() {
    let typedDate = this.getDate();
    let typedAmount = this.getAmount();
    let expenseDTO: NewExpenseDTO = {
      description : this.descriptionElement.nativeElement.value,
      amount : typedAmount,
      date : typedDate,
    };
    this.expensesService.createExpense(expenseDTO).subscribe(() => {
      this.modalElement.nativeElement.close();
    });
  }
}
