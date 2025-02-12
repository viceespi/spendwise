import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { NewExpenseDTO } from '../models/newExpenseDTO';

@Injectable({
  providedIn: null,
})
export class ExpensesService {
  constructor(private http: HttpClient) {}

  createExpense(newExpenseDTO: NewExpenseDTO): Observable<void> {
    return this.http.post<void>('http://localhost:5029/expenses', newExpenseDTO);
  }

  updateExpense() {}

  getExpense() {}

  getAllExpenses() {}
}
