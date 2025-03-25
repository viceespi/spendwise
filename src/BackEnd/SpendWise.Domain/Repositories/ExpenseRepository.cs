using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Repositories.Contracts;

namespace SpendWise.Domain.Repositories
{
    [ExcludeFromCodeCoverage(Justification = "The IDbConnection query funcions are extention methods, and can't be mocked.")]

    public class ExpenseRepository : IExpenseRepository
    {
        private readonly IDbConnection _connection;

        public ExpenseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Expense> CreateNewExpense(Expense expenseDTO)
        {
            const string sqlOrder =
            @"
                INSERT
                INTO
                expenses
                (description, date, amount)
                VALUES
                (@Description, @Date, @Amount)
                RETURNING
                expense_id;
            ";

            Guid expenseId = await _connection.QueryFirstAsync<Guid>(sqlOrder, new
            {
                Description = expenseDTO.Description,
                Date = expenseDTO.Date,
                Amount = expenseDTO.Amount
            });
            Expense newExpense = new(expenseDTO.Description, expenseDTO.Date, expenseDTO.Amount, expenseId);

            return newExpense;
        }

        public async Task DeleteExpense(Guid expenseId)
        {
            const string sqlOrder =
            @"
                DELETE
                FROM
                expenses
                WHERE
                expense_id = @Id;
            ";

            await _connection.ExecuteAsync(sqlOrder, new
            {
                Id = expenseId
            });
        }

        public async Task<List<Expense>> GetAllExpenses()
        {
            const string sqlOrder =
            @"
                SELECT
                description AS Description,
                date AS Date,
                amount AS Amount,
                expense_id AS Id
                FROM
                expenses
                ORDER BY date DESC;
            ";

            List<Expense> expenses = (await _connection.QueryAsync<Expense>(sqlOrder)).ToList();
            return expenses;
        }

        public async Task<Expense?> GetExpense(Guid expenseId)
        {
            const string sqlOrder =
            @"
                SELECT
                *
                FROM
                expenses
                WHERE
                expense_id = @Id
            ";

            Expense? expenseDTO = await _connection.QueryFirstOrDefaultAsync<Expense>(sqlOrder, new
            {
                Id = expenseId
            });
            return expenseDTO;
        }

        public async Task UpdateExpense(Expense toUpdateExpense)
        {
            const string sqlOrder =
            @"
                UPDATE expenses
                SET 
                expense_id = @ExpenseId,
                description = @Description,
                date = @Date,
                amount = @Amount
                WHERE 
                expense_id = @ExpenseId;
            ";

            await _connection.ExecuteAsync(sqlOrder, new
            {
                Description = toUpdateExpense.Description,
                Date = toUpdateExpense.Date,
                Amount = toUpdateExpense.Amount,
                ExpenseId = toUpdateExpense.Id
            });
        }
    }
}