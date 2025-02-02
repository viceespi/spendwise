using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;

namespace SpendWise.Domain.Validators.Contracts
{
    public interface IExpenseValidator
    {
        public ValidationErrors Validate(Expense expense);
        
    }
}