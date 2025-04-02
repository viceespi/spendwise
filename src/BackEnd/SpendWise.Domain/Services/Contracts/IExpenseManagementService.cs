using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;

namespace SpendWise.Domain.Services.Contracts
{
    public interface IExpenseManagementService
    {
        Task<Result<Expense>> CreateExpense(NewExpenseDto newExpenseDto);

        Task<List<Expense>> GetAllExpenses(Guid ownerId);

        Task<Expense?> GetExpense(Guid expenseId);

        Task DeleteExpense(Guid expenseId);

        Task<Result<Expense>> UpdateExpense(ToUpdateExpenseDto toUpdateExpenseDto);
    }
}