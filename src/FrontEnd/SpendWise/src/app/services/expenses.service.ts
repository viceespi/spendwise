import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Expense } from '../models/Expense';
import { ToUpdateExpenseDto } from '../models/ToUpdateExpenseDto';
import { NewExpenseDto } from '../models/NewExpenseDto';

@Injectable({
  providedIn: null,
})
export class ExpensesService {
  constructor(private http: HttpClient) {}

  CreateExpense(newExpenseDto: NewExpenseDto): Observable<Expense> {
    return this.http.post<Expense>(
      'http://localhost:5029/expenses',
      newExpenseDto
    );
  }

  DeleteExpense(expenseId: string): Observable<void> {
    return this.http.delete<void>(
      `http://localhost:5029/expenses/${expenseId}`
    );
  }

  UpdateExpense(toUpdateExpenseDto: ToUpdateExpenseDto): Observable<Expense> {
    return this.http.put<Expense>(
      'http://localhost:5029/expenses',
      toUpdateExpenseDto
    );
  }

  GetExpense() {}

  GetAllExpenses(ownerId: string): Observable<Expense[]> {
    return this.http.get<Expense[]>(
      `http://localhost:5029/expenses?ownerId=${ownerId}`
    );
  }
}
