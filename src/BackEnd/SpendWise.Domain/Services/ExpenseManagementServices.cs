using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Factories.Contracts;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Repositories.Contracts;
using SpendWise.Domain.Services.Contracts;

namespace SpendWise.Domain.Services
{
    public class ExpenseManagementServices : IExpenseManagementServices
    {
        private readonly IExpenseRepository _repository;
        
        private readonly IExpenseFactory _factory;

        public ExpenseManagementServices(IExpenseFactory factory, IExpenseRepository repository)
        {
            this._factory = factory;
            this._repository = repository;
        }

        public async Task<Result<Guid>> CreateExpense(NewExpenseDTO newExpenseDTO)
        {
            Result<Expense> newExpenseResult = _factory.CreateExpenseFromNewExpenseDTO(newExpenseDTO);
            return await newExpenseResult.Match
            (
                async (expense) =>
                {
                    Guid expenseId = await _repository.CreateNewExpense(expense);
                    Result<Guid> successfullInputResult = new Result<Guid>(expenseId);
                    return successfullInputResult;
                }
                ,
                (validationResult) =>
                {
                    var failedInputResult = new Result<Guid>(validationResult);
                    return Task.FromResult(failedInputResult);
                }
            );
        }

        public async Task<List<Expense>> GetAllExpenses()
        {
            List<Expense> expenses = await _repository.GetAllExpenses();
            return expenses;
        }

        public async Task<Expense?> GetExpense(Guid expenseId)
        {
            Expense? expense = await _repository.GetExpense(expenseId);
            return expense;
        }
    }
}