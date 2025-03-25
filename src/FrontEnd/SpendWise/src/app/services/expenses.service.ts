import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { NewExpenseDTO } from '../models/NewExpenseDTO';
import { Expense } from '../models/Expense';
import { ToUpdateExpenseDTO } from '../models/ToUpdateExpenseDTO';

@Injectable({
  providedIn: null,
})
export class ExpensesService {
  constructor(private http: HttpClient) {}

  CreateExpense(newExpenseDTO: NewExpenseDTO): Observable<Expense> {
    return this.http.post<Expense>(
      'http://localhost:5029/expenses',
      newExpenseDTO
    );
  }

  DeleteExpense(expenseId: string): Observable<void> {
    return this.http.delete<void>(
      `http://localhost:5029/expenses/${expenseId}`
    );
  }

  UpdateExpense(toUpdateExpenseDTO: ToUpdateExpenseDTO): Observable<Expense> {
    return this.http.put<Expense>(
      'http://localhost:5029/expenses',
      toUpdateExpenseDTO
    );
  }

  GetExpense() {}

  GetAllExpenses(): Observable<Expense[]> {
    return this.http.get<Expense[]>('http://localhost:5029/expenses');
  }
}
