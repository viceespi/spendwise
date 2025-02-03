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

        public async Task<Guid> CreateNewExpense(Expense expense)
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
                id;
            ";

            Guid expenseId = await _connection.QueryFirstAsync<Guid>(sqlOrder, new
            {
                Description = expense.Description,
                Date = expense.Date,
                Amount = expense.Amount
            });
            return expenseId;
        }

        public async Task<List<Expense>> GetAllExpenses()
        {
            const string sqlOrder =
            @"
                SELECT
                *
                FROM
                expenses;
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

            Expense? expense = await _connection.QueryFirstOrDefaultAsync<Expense>(sqlOrder, new
            {
                Id = expenseId
            });
            return expense;
        }
    }
}