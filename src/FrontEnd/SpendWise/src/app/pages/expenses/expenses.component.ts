import {
  Component,
  computed,
  effect,
  ElementRef,
  output,
  signal,
  ViewChild,
} from '@angular/core';
import { ActionBarComponent } from '../../components/expense-components/action-bar/action-bar.component';
import { InfoModalComponent } from '../../components/general-components/info-modal/info-modal.component';
import { SubmitModalComponent } from '../../components/general-components/submit-modal/submit-modal.component';
import { NewExpenseDto } from '../../models/NewExpenseDto';
import { ExpensesService } from '../../services/expenses.service';
import { Expense } from '../../models/Expense';
import { ExpenseBoardComponent } from '../../components/expense-components/expense-board/expense-board.component';
import { ButtonComponent } from '../../components/general-components/button/button.component';
import { ToUpdateExpenseDto } from '../../models/ToUpdateExpenseDto';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-expenses',
  imports: [
    ActionBarComponent,
    InfoModalComponent,
    SubmitModalComponent,
    ExpenseBoardComponent,
    ButtonComponent,
  ],
  templateUrl: './expenses.component.html',
  styleUrl: './expenses.component.css',
  providers: [ExpensesService],
})
export class ExpensesComponent {
  constructor(
    private expensesService: ExpensesService,
    private sessionService: SessionService
  ) {}

  currentUserId = computed(() => this.sessionService.currentUser()?.id!);

  // EXPENSE CREATION MODAL LOGIC --------------------------------------------------------------------------------------------------------

  formatDate(date: Date): string[] {
    let month = '';
    if (date.getMonth() < 10) {
      month = (date.getMonth() + 1).toString().padStart(2, '0');
    } else {
      month = (date.getMonth() + 1).toString();
    }
    return [
      date.getFullYear().toString(),
      month,
      date.getDate().toString().padStart(2, '0'),
    ];
  }

  // CREATE

  currentDate = signal(new Date());

  formatedCurrentDate = computed(() => {
    let date = this.currentDate();
    return this.formatDate(date);
  });

  @ViewChild('newExpenseDescription') newExpenseDescriptionElement!: ElementRef;
  @ViewChild('newExpenseAmount') newExpenseAmountElement!: ElementRef;
  @ViewChild('newExpenseDate') newExpenseDateElement!: ElementRef;

  newExpenseDtoSignal = signal<NewExpenseDto | null>(null);

  createNewExpenseDTO() {
    let description = this.newExpenseDescriptionElement.nativeElement.value;
    let date = new Date(this.newExpenseDateElement.nativeElement.value);
    let amount = Number(this.newExpenseAmountElement.nativeElement.value);
    let newExpenseDto: NewExpenseDto = {
      description: description,
      amount: amount,
      date: date,
      ownerId: this.currentUserId(),
    };
    this.newExpenseDtoSignal.set(newExpenseDto);
  }

  clearExpenseCreationCache() {
    this.newExpenseDescriptionElement.nativeElement.value = '';
    this.newExpenseAmountElement.nativeElement.value = '0';
  }

  createIsSuccessfull = signal<boolean>(false);

  expenseCreationErrors = signal<string[]>([]);

  // DELETE

  toDeleteExpenseId = signal<string | null>(null);

  deleteIsSuccessfull = signal<boolean>(false);

  // UPDATE

  toUpdateExpense = signal<Expense | null>(null);

  toUpdateExpenseDate = computed(() => {
    if (this.toUpdateExpense() !== null) {
      return this.toUpdateExpense()?.date;
    }
    return this.currentDate();
  });

  formatedToUpdateExpenseDate = computed(() => {
    let date = this.toUpdateExpenseDate();
    return this.formatDate(date!);
  });

  @ViewChild('toUpdateExpenseDescription')
  toUpdateExpenseDescriptionElement!: ElementRef;
  @ViewChild('toUpdateExpenseAmount') toUpdateExpenseAmountElement!: ElementRef;
  @ViewChild('toUpdateExpenseDate') toUpdateExpenseDateElement!: ElementRef;

  toUpdateExpenseDtoSignal = signal<ToUpdateExpenseDto | null>(null);

  createToUpdateExpenseDto() {
    let description =
      this.toUpdateExpenseDescriptionElement.nativeElement.value;
    let date = new Date(this.toUpdateExpenseDateElement.nativeElement.value);
    let amount = Number(this.toUpdateExpenseAmountElement.nativeElement.value);
    let id = this.toUpdateExpense()?.id!;
    let ownerId = this.toUpdateExpense()?.ownerId!;
    let toUpdateExpenseDto: ToUpdateExpenseDto = {
      description: description,
      amount: amount,
      date: date,
      id: id,
      ownerId,
    };
    this.toUpdateExpenseDtoSignal.set(toUpdateExpenseDto);
  }

  updateIsSuccessfull = signal<boolean>(false);
  expenseUpdateErrors = signal<string[]>([]);
}
