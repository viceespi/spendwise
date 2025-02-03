using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;

namespace SpendWise.Domain.Services.Contracts
{
    public interface IExpenseManagementServices
    {
        Task<Result<Guid>> CreateExpense(NewExpenseDTO newExpenseDTO);

        Task<List<Expense>> GetAllExpenses();

        Task<Expense?> GetExpense(Guid expenseId); 

        Task DeleteExpense(Guid expenseId);
    }
}