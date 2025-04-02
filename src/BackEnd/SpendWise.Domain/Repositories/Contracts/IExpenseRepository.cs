using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Models.ExpenseModels;

namespace SpendWise.Domain.Repositories.Contracts
{
    public interface IExpenseRepository
    {
        Task<Expense> CreateNewExpense(Expense expenseDTO);

        Task<List<Expense>> GetAllExpenses(Guid ownerId);

        Task<Expense?> GetExpense(Guid expenseId);

        Task DeleteExpense(Guid expenseId);

        Task UpdateExpense(Expense toUpdateExpense);
    }
}