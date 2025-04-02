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

        public Result<Expense> CreateExpenseFromNewExpenseDto(NewExpenseDto newExpenseDto)
        {
            Expense newExpense = new(newExpenseDto.Description, newExpenseDto.Date, newExpenseDto.Amount, Guid.Empty,  newExpenseDto.OwnerId);
            ValidationErrors expenseValidation = this._validator.Validate(newExpense);
            if (expenseValidation.HasError)
            {
                Result<Expense> failedResult    = new (expenseValidation); 
                return failedResult;
            }
            Result<Expense> successResult = new(newExpense);
            return successResult;
        }

        public Result<Expense> CreateExpenseFromToUpdateExpenseDto(ToUpdateExpenseDto toUpdateExpenseDto)
        {
            Expense updatedExpense = new(toUpdateExpenseDto.Description, toUpdateExpenseDto.Date, toUpdateExpenseDto.Amount, toUpdateExpenseDto.Id, toUpdateExpenseDto.OwnerId);
            ValidationErrors expenseValidation = this._validator.Validate(updatedExpense);
            if (expenseValidation.HasError)
            {
                Result<Expense> failedResult    = new (expenseValidation); 
                return failedResult;
            }
            Result<Expense> successResult = new(updatedExpense);
            return successResult;
        }

    }
    
}