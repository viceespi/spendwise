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
    public class ExpenseManagementService : IExpenseManagementService
    {
        private readonly IExpenseRepository _repository;

        private readonly IExpenseFactory _factory;

        public ExpenseManagementService(IExpenseFactory factory, IExpenseRepository repository)
        {
            this._factory = factory;
            this._repository = repository;
        }

        public async Task<Result<Expense>> CreateExpense(NewExpenseDto newExpenseDto)
        {
            Result<Expense> newExpenseResult = _factory.CreateExpenseFromNewExpenseDto(newExpenseDto);
            return await newExpenseResult.Match
            (
                async (expenseDto) =>
                {
                    Expense newExpense = await _repository.CreateNewExpense(expenseDto);
                    Result<Expense> successfullInputResult = new Result<Expense>(newExpense);
                    return successfullInputResult;
                }
                ,
                (validationErrors) =>
                {
                    var failedInputResult = new Result<Expense>(validationErrors);
                    return Task.FromResult(failedInputResult);
                }
            );
        }

        public async Task DeleteExpense(Guid expenseId)
        {
            await _repository.DeleteExpense(expenseId);
        }

        public async Task<List<Expense>> GetAllExpenses(Guid ownerId)
        {
            List<Expense> expenses = await _repository.GetAllExpenses(ownerId);
            return expenses;
        }

        public async Task<Expense?> GetExpense(Guid expenseId)
        {
            Expense? expense = await _repository.GetExpense(expenseId);
            return expense;
        }

        public async Task<Result<Expense>> UpdateExpense(ToUpdateExpenseDto toUpdateExpenseDto)
        {
            Result<Expense> newExpenseResult = _factory.CreateExpenseFromToUpdateExpenseDto(toUpdateExpenseDto);
            return await newExpenseResult.Match
            (
                async (expense) =>
                {
                    await _repository.UpdateExpense(expense);
                    Result<Expense> result = new(expense);
                    return result;
                }
                ,
                (validationErrors) =>
                {
                    var failedInputResult = new Result<Expense>(validationErrors);
                    return Task.FromResult(failedInputResult);
                }
            );
        }
    }
}