using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Factories.Contracts;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Validators.Contracts;

namespace SpendWise.Domain.Factories
{
    public class ExpenseFactory : IExpenseFactory
    {
        private IExpenseValidator _validator { get; }

        public ExpenseFactory(IExpenseValidator validator)
        {
            this._validator = validator;
        }

        public Result<Expense> CreateExpenseFromNewExpenseDTO(NewExpenseDTO expense)
        {
            Expense newExpense = new(expense.Description, expense.Date, expense.Amount, Guid.Empty);
            ValidationErrors expenseValidation = this._validator.Validate(newExpense);
            if (expenseValidation.HasError)
            {
                Result<Expense> failedInputResult = new (expenseValidation); 
                return failedInputResult;
            }
            Result<Expense> successfullInputResult = new(newExpense);
            return successfullInputResult;
        }

    }
    
}