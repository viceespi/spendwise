import {
  Component,
  computed,
  signal,
  OnInit,
  output,
  input,
  effect,
} from '@angular/core';
import { ExpenseLineComponent } from '../expense-line/expense-line.component';
import { ExpensesService } from '../../../services/expenses.service';
import { Expense } from '../../../models/Expense';
import { NewExpenseDto } from '../../../models/NewExpenseDto';
import { ToUpdateExpenseDto } from '../../../models/ToUpdateExpenseDto';
import { HttpErrorResponse } from '@angular/common/http';
import { SessionService } from '../../../services/session.service';

@Component({
  selector: 'app-expense-board',
  imports: [ExpenseLineComponent],
  templateUrl: './expense-board.component.html',
  styleUrl: './expense-board.component.css',
})
export class ExpenseBoardComponent {
  constructor(
    private expensesService: ExpensesService,
    private sessionService: SessionService
  ) {}

  currentUserId = computed<string>(
    () => this.sessionService.currentUser()?.id!
  );

  // EXPENSE FETCH

  expenseFetchErrorSignal = output<void>();
  expensesList = signal<Expense[]>([]);
  ngOnInit() {
    this.expensesService.GetAllExpenses(this.currentUserId()).subscribe({
      next: (response) => {
        let newExpenseList: Expense[] = [];
        for (const object of response) {
          let newExpense: Expense = {
            description: object.description,
            amount: object.amount,
            date: new Date(object.date),
            id: object.id,
            ownerId: object.ownerId,
          };
          console.log(newExpense.date);
          newExpenseList.push(newExpense);
        }
        this.expensesList.update((expensesList) => newExpenseList);
      },

      error: (err) => {
        this.expenseFetchErrorSignal.emit();
      },
    });
  }

  // dar um jeito de fazer o get all toda vez que a página carrega => DONE
  // dar um jeito de adicionar uma expense que vem do componente pai (pagina expense) no mapa de expense
  // dar um jeito de rerenderizar o @for toda vez que a quantidade de items no mapa muda.
  // ~~ lembrar de implementar a lógica de renderização do valor e data do jeito que eu quero => DONE

  // EXPENSE DELETE

  expenseDeleteClicked = output<string>();
  expenseDeleteSuccessfull = output<void>();
  expenseDeleteFailed = output<void>();

  DeleteExpenseCall(toDeleteExpenseId: string) {
    this.expensesService.DeleteExpense(toDeleteExpenseId).subscribe({
      next: () => {
        let updatedExpensesList = this.expensesList();
        for (let i = 0; i < updatedExpensesList.length; i++) {
          if (updatedExpensesList[i].id === toDeleteExpenseId) {
            updatedExpensesList.splice(i, 1);
          }
        }
        this.expensesList.update((expensesList) => updatedExpensesList);
        this.expenseDeleteSuccessfull.emit();
      },
      error: () => {
        this.expenseDeleteFailed.emit();
      },
    });
  }

  // EXPENSE UPDATE

  expenseUpdateClicked = output<Expense>();
  expenseUpdateErrors = signal<string[]>([]);
  expenseUpdateSuccessfull = output<void>();
  expenseUpdateFailed = output<string[]>();

  UpdateExpenseCall(toUpdateExpenseDto: ToUpdateExpenseDto) {
    this.expensesService.UpdateExpense(toUpdateExpenseDto).subscribe({
      next: (object) => {
        let updatedExpense: Expense = {
          description: object.description,
          amount: object.amount,
          date: new Date(object.date),
          id: object.id,
          ownerId: object.ownerId,
        };
        let updatedExpensesList = this.expensesList();
        for (let i = 0; i < updatedExpensesList.length; i++) {
          if (updatedExpensesList[i].id === updatedExpense.id) {
            updatedExpensesList[i] = updatedExpense;
          }
        }
        updatedExpensesList.sort(
          (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
        );
        this.expensesList.update((expensesList) => updatedExpensesList);
        this.expenseUpdateErrors.set([]);
        this.expenseUpdateSuccessfull.emit();
      },
      error: (errors: HttpErrorResponse) => {
        this.expenseCreationErrors.set([]);
        let statusCode = errors.status;
        if (statusCode === 400) {
          this.expenseUpdateErrors.set(errors.error);
        } else {
          let serviceError: string[] = ['Server error!'];
          this.expenseUpdateErrors.set(serviceError);
        }
        this.expenseUpdateFailed.emit(this.expenseUpdateErrors());
      },
    });
  }

  // EXPENSE CREATION

  expenseCreationErrors = signal<string[]>([]);
  expenseCreationSucessfull = output<void>();
  expenseCreationFailed = output<string[]>();

  CreateExpenseCall(newExpenseDto: NewExpenseDto) {
    this.expensesService.CreateExpense(newExpenseDto).subscribe({
      next: (object) => {
        this.expenseCreationErrors.set([]);
        let newExpense: Expense = {
          description: object.description,
          amount: object.amount,
          date: new Date(
            Date.UTC(
              Number(object.date.toString().substring(0, 4)),
              Number(object.date.toString().substring(5, 7)) - 1,
              Number(object.date.toString().substring(8, 10)),
              3,
              0,
              0
            )
          ),
          id: object.id,
          ownerId: object.ownerId,
        };

        console.log(`Expense criada data: ${newExpense.date}`);
        let updatedExpensesList = this.expensesList();
        updatedExpensesList.push(newExpense);
        updatedExpensesList.sort(
          (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
        );
        this.expensesList.update((expensesList) => updatedExpensesList);
        this.expenseCreationSucessfull.emit();
      },
      error: (errors: HttpErrorResponse) => {
        this.expenseCreationErrors.set([]);
        let statusCode = errors.status;
        if (statusCode === 400) {
          this.expenseCreationErrors.set(errors.error);
        } else {
          let serviceError: string[] = ['Server error!'];
          this.expenseCreationErrors.set(serviceError);
        }
        this.expenseCreationFailed.emit(this.expenseCreationErrors());
      },
    });
  }
}
