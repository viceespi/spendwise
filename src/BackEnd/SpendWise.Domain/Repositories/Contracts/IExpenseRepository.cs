using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Models.ExpenseModels;

namespace SpendWise.Domain.Repositories.Contracts
{
    public interface IExpenseRepository
    {
        Task<Guid> CreateNewExpense(Expense expense);
    }
}