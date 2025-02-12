using Dapper;
using Npgsql;
using Popoupa.Core.DataBaseManipulation.Interfaces;
using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.Repositories
{
    public class BankStatementRepository : IBankStatementRepositoryInterface
    {
        private readonly string popoupaDB = "Server=192.168.0.21;Port=5432;Database=popoupa;User Id=postgres;Password=Nina100%";

        public async Task<Guid> Add(BankStatement newBankStatement)
        {
            const string sqlOrder = @"
                INSERT INTO
                bank_statements (bank_name, submission_date, month_of_reference, owner_id)
                VALUES
                (@BankName, @SubmissionDate, @MonthOfReference, @OwnerId)
                RETURNING bank_statement_id;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var bankStatementId = await popoupaDBConnection.QuerySingleAsync<Guid>(sqlOrder, new
                    {
                        BankName = newBankStatement.BankName,
                        SubmissionDate = newBankStatement.SubmissionDate,
                        MonthOfReference = newBankStatement.MonthOfReference,
                        OwnerId = newBankStatement.OwnerId
                    });

                    return bankStatementId;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during bank statement creation query: {exception.Message}");
                }
            }
        }

        public async Task Delete(Guid bankStatementId)
        {
            const string sqlOrder = @"
                DELETE
                FROM
                bank_statements
                WHERE
                bank_statement_id = @BankStatementId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        BankStatementId = bankStatementId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during bank statement deletion query: {exception.Message}");
                }
            }
        }

        public async Task<BankStatement?> Get(Guid bankStatementId)
        {
            const string sqlOrder = @"
                SELECT
                bank_statement_id AS Id,
                bank_name AS BankName,
                submission_date AS SubmissionDate,
                month_of_reference AS MonthOfReference,
                owner_id AS OwnerId
                FROM
                bank_statements
                WHERE
                bank_statement_id = @BankStatementId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var bankStatement = await popoupaDBConnection.QuerySingleAsync<BankStatement>(sqlOrder, new
                    {
                        BankStatementId = bankStatementId
                    });

                    return bankStatement;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during bank statement query: {exception.Message}");
                }
            }
        }

        public async Task<List<BankStatement?>> GetAll(Guid userId)
        {
            const string sqlOrder = @"
                SELECT
                bank_statement_id AS Id,
                bank_name AS BankName,
                submission_date AS SubmissionDate,
                month_of_reference AS MonthOfReference,
                owner_id AS OwnerId
                FROM
                bank_statements
                WHERE
                owner_id = @UserId; 
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var bankStatements = await popoupaDBConnection.QueryAsync<BankStatement?>(sqlOrder, new
                    {
                        UserId = userId
                    });

                    return bankStatements.ToList();
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during bank statements query: {exception.Message}");
                }
            }
        }

        public async Task Update(BankStatement bankStatement)
        {
            const string sqlOrder = @"
                UPDATE
                bank_statements
                SET
                bank_statement_id = @BankStatementId,
                bank_name = @BankName,
                submission_date = @SubmissionDate,
                month_of_reference = @MonthOfReference,
                owner_id = @OwnerId
                WHERE
                bank_statement_id = @BankStatementId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        BankStatementId = bankStatement.Id,
                        BankName = bankStatement.BankName,
                        SubmissionDate = bankStatement.SubmissionDate,
                        MonthOfReference = bankStatement.MonthOfReference,
                        OwnerId = bankStatement.OwnerId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during bank statement update query: {exception.Message}");
                }
            }
        }

        public async Task CreateUserBankStatementRelation(Guid userId, Guid bankStatementId)
        {
            const string sqlOrder = @"
                INSERT INTO
                user_bank_statements (user_id, bank_statement_id)
                VALUES
                (@UserId, @BankStatementId);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        UserId = userId,
                        BankStatementId = bankStatementId
                    });

                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during user bank statement relation creation query: {exception.Message}");
                }
            }
        }

        public async Task CreateBankStatementExpenseRelation(Guid bankStatementId, List<Guid> expensesIds)
        {
            const string sqlOrder = @"
                INSERT INTO
                bank_statement_expenses (bank_statement_id, expense_id)
                VALUES
                (@BankStatementId, @ExpenseId);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                await popoupaDBConnection.OpenAsync();
                using (var popoTransaction = popoupaDBConnection.BeginTransaction())
                {
                    try
                    {
                        foreach (Guid expenseId in expensesIds)
                        {
                            await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                            {
                                BankStatementId = bankStatementId,
                                ExpenseId = expenseId
                            });
                        }
                        await popoTransaction.CommitAsync();
                        await popoupaDBConnection.CloseAsync();
                    }
                    catch (InvalidOperationException)
                    {
                        await popoTransaction.RollbackAsync();
                        throw new Exception("Error in the PostgresDB! Error during bank statement expenses relation creation query! The transaction was rolledBack!");
                    }
                    catch (Exception exception)
                    {
                        await popoTransaction.RollbackAsync();
                        throw new Exception($"Error in the PostgresDB! Error during bank statement expense relation creation query: {exception.Message}");
                    }
                }
            }
        }
    }
}
