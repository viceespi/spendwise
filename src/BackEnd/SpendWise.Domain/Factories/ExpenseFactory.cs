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

        public Result<Expense> CreateExpenseFromNewExpenseDTO(NewExpenseDTO newExpenseDTO)
        {
            Expense newExpense = new(newExpenseDTO.Description, newExpenseDTO.Date, newExpenseDTO.Amount, Guid.Empty);
            ValidationErrors expenseValidation = this._validator.Validate(newExpense);
            if (expenseValidation.HasError)
            {
                Result<Expense> failedInputResult = new (expenseValidation); 
                return failedInputResult;
            }
            Result<Expense> successfullInputResult = new(newExpense);
            return successfullInputResult;
        }

        public Result<Expense> CreateExpenseFromToUpdateExpenseDTO(ToUpdateExpenseDTO toUpdateExpenseDTO)
        {
            Expense updatedExpense = new(toUpdateExpenseDTO.Description, toUpdateExpenseDTO.Date, toUpdateExpenseDTO.Amount, toUpdateExpenseDTO.Id);
            ValidationErrors expenseValidation = this._validator.Validate(updatedExpense);
            if (expenseValidation.HasError)
            {
                Result<Expense> failedInputResult = new (expenseValidation); 
                return failedInputResult;
            }
            Result<Expense> successfullInputResult = new(updatedExpense);
            return successfullInputResult;
        }

    }
    
}