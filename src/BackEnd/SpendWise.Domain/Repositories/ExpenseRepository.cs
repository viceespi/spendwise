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
                (description, date, amount, owner_id)
                VALUES
                (@Description, @Date, @Amount, @OwnerId)
                RETURNING
                expense_id;
            ";

            Guid expenseId = await _connection.QueryFirstAsync<Guid>(sqlOrder, new
            {
                Description = expenseDTO.Description,
                Date = expenseDTO.Date,
                Amount = expenseDTO.Amount,
                OwnerId = expenseDTO.OwnerId
            });
            Expense newExpense = new(expenseDTO.Description, expenseDTO.Date, expenseDTO.Amount, expenseId, expenseDTO.OwnerId);

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

        public async Task<List<Expense>> GetAllExpenses(Guid ownerId)
        {
            const string sqlOrder =
            @"
                SELECT
                description AS Description,
                date AS Date,
                amount AS Amount,
                expense_id AS Id,
                owner_id AS OwnerId
                FROM
                expenses
                WHERE
                owner_id = @OwnerId
                ORDER BY date DESC;
            ";

            List<Expense> expenses = (await _connection.QueryAsync<Expense>(sqlOrder, new { OwnerId = ownerId})).ToList();
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
                amount = @Amount,
                owner_id = @OwnerId
                WHERE 
                expense_id = @ExpenseId;
            ";

            await _connection.ExecuteAsync(sqlOrder, new
            {
                Description = toUpdateExpense.Description,
                Date = toUpdateExpense.Date,
                Amount = toUpdateExpense.Amount,
                ExpenseId = toUpdateExpense.Id,
                OwnerId = toUpdateExpense.OwnerId
            });
        }
    }
}