using System.Text;
using Popoupa.Core.DomainModels;
using Popoupa.Core.DataBaseManipulation.Interfaces;
using Popoupa.Core.DataBaseManipulation.Filters;
using Dapper;
using Npgsql;
using Popoupa.Core.DataBaseManipulation.DBClasses;
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace Popoupa.Core.DataBaseManipulation
{
    public class ExpenseRepository : IExpenseRepositoryInterface
    {
        private readonly string popoupaDB = "Server=192.168.0.21;Port=5432;Database=popoupa;User Id=postgres;Password=Nina100%";

        public async Task<Guid> Add(Expense newExpense)
        {
            const string sqlOrder = @"
                INSERT INTO
                expenses(description, amount, date, expense_category, expense_group, owner_id, is_shared)
                VALUES
                (@Description, @Amount, @Date, @ExpenseCategory, @ExpenseGroup, @OwnerId, false)
                RETURNING expense_id;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var expenseId = await popoupaDBConnection.QueryFirstAsync<Guid>(sqlOrder, new
                    {
                        Description = newExpense.Description,
                        Amount = newExpense.Amount,
                        Date = newExpense.Date,
                        ExpenseCategory = newExpense.Category.Id,
                        ExpenseGroup = newExpense.Group.Id,
                        OwnerId = newExpense.OwnerId
                    });

                    return expenseId;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during expense creation query: {exception.Message}");
                }
            }
        }

        public async Task<List<Guid>> AddMultiple(List<Expense> newExpenses)
        {
            var expensesIds = new Guid[newExpenses.Count];
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                await popoupaDBConnection.OpenAsync();
                try
                {
                    using (var popoTransaction = await popoupaDBConnection.BeginTransactionAsync())
                    {
                        try
                        {
                            for (var index = 0; index < newExpenses.Count; index++)
                            {
                                var expenseId = await Add(newExpenses[index]);
                                expensesIds[index] = expenseId;
                            }
                            await popoTransaction.CommitAsync();
                            await popoupaDBConnection.CloseAsync();
                            return expensesIds.ToList();
                        }
                        catch (Exception exception)
                        {
                            popoTransaction.Rollback();
                            throw new Exception($"Error in the PostgresDB! Error during multiple expenses creation query! The transaction was rolledBack! Error: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during multiple expenses creation transaction creation: {exception.Message}");
                }
            }
        }
        public async Task CreateMultipleOwnerExpenseRelation(Guid userId, List<Guid> expensesIds)
        {
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                await popoupaDBConnection.OpenAsync();
                try
                {
                    using (var popoTransaction = await popoupaDBConnection.BeginTransactionAsync())
                    {
                        try
                        {
                            foreach (Guid expenseId in expensesIds)
                            {
                                await CreateOwnerExpenseRelation(userId, expenseId);
                            }
                            await popoTransaction.CommitAsync();
                            await popoupaDBConnection.CloseAsync();
                        }
                        catch (Exception exception)
                        {
                            popoTransaction.Rollback();
                            throw new Exception($"Error in the PostgresDB! Error during multiple owner expenses relation creation query! The transaction was rolledBack! Error: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during multiple owner expense relations transaction creation: {exception.Message}");
                }
            }
        }

        public async Task CreateOwnerExpenseRelation(Guid userId, Guid expenseId)
        {
            const string sqlOrder = @"
            INSERT INTO
            user_expenses_shares (user_id, expense_id, share)
            VALUES
            (@UserId, @ExpenseId, 1.0);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        UserId = userId,
                        ExpenseId = expenseId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during owner expense relation creation query: {exception.Message}");
                }
            }
        }

        public async Task Delete(Guid expenseId)
        {
            const string sqlOrder = @"
            DELETE
            FROM
            expenses
            WHERE
            expense_id = @ExpenseId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        ExpenseId = expenseId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during expense deletion query: {exception.Message}");
                }
            }
        }

        public async Task DeleteMultiple(List<Guid> expensesIds)
        {
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                await popoupaDBConnection.OpenAsync();
                try
                {
                    using (var popoTransaction = await popoupaDBConnection.BeginTransactionAsync())
                    {
                        try
                        {
                            foreach (Guid expenseId in expensesIds)
                            {
                                await Delete(expenseId);
                            }
                            await popoTransaction.CommitAsync();
                            await popoupaDBConnection.CloseAsync();
                        }
                        catch (Exception exception)
                        {
                            popoTransaction.Rollback();
                            throw new Exception($"Error in the PostgresDB! Error during expenses deletion query! The transaction was rolledBack! Error: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during multiple expenses deletion query: {exception.Message}");
                }
            }
        }

        public async Task DeleteShareFromExpense(List<Guid> usersIds, Guid expenseId)
        {
            const string sqlOrder = @"
                DELETE
                FROM
                user_expenses_shares
                WHERE
                user_id IN @UserIds
                AND
                expense_id = @ExpenseId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        UserIds = usersIds,
                        ExpenseId = expenseId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during expense sharing deletion query: {exception.Message}");
                }
            }
        }

        public async Task<Expense?> Get(Guid expenseId, Guid userId)
        {
            const string expenseSQLOrder = @"
                SELECT
                expense_id AS Id,
                description AS Description,
                amount AS Amount,
                date AS Date,
                expense_category AS Category,
                expense_group AS Group,
                owner_id AS OwnerId,
                is_shared As IsShared
                FROM
                expenses
                WHERE
                expense_id = @ExpenseId;
            ";

            const string categorySQLOrder = @"
                SELECT
                category_id AS Id,
                name AS Name
                FROM
                categories
                WHERE
                category_id = @CategoryId
            ";

            const string groupSQLOrder = @"
                SELECT
                groups.group_id AS Id,
                groups.name AS Name
                FROM
                groups
                WHERE
                group_id = @GroupId
            ";

            const string shareSQLOrder = @"
                SELECT
                share
                FROM 
                user_expenses_shares
                WHERE
                user_id = @UserId
                AND
                expense_id = @ExpenseId

            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var expenseEntity = await popoupaDBConnection.QueryFirstAsync<ExpenseEntity>(expenseSQLOrder, new
                    {
                        ExpenseId = expenseId
                    });
                    var expenseCategory = await popoupaDBConnection.QueryFirstAsync<Category>(categorySQLOrder, new
                    {
                        CategoryId = expenseEntity.ExpenseCategoryId
                    });
                    var expenseGroup = await popoupaDBConnection.QueryFirstAsync<Group>(groupSQLOrder, new
                    {
                        GroupId = expenseEntity.ExpenseGroupId
                    });
                    var share = await popoupaDBConnection.QueryFirstAsync<decimal>(shareSQLOrder, new
                    {
                        UserId = userId,
                        ExpenseId = expenseId
                    });

                    var domainExpense = new Expense(expenseEntity.Id, expenseEntity.Description, expenseEntity.Amount, expenseEntity.Date, expenseCategory, expenseGroup, expenseEntity.IsShared, expenseEntity.OwnerId);

                    return domainExpense;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during expense - query: {exception.Message}");
                }
            }
        }

        public async Task<List<Expense?>> GetFromBankStatement(Guid bankStatementId, Guid userId)
        {
            const string sqlOrder =
            @"
                SELECT
                    expenses.expense_id AS Id,
                    expenses.description AS Description,
                    expenses.amount AS Amount,
                    expenses.date AS Date,
                    expenses.expense_category AS Category,
                    expenses.expense_group AS Group,
                    expenses.owner_id AS OwnerId,
                    expenses.is_shared AS IsShared
                FROM
                    expenses
                INNER JOIN
                    bank_statement_expenses
                ON
                    expenses.expense_id = bank_statement_expenses.expense_id
                WHERE
                    bank_statement_expenses.bank_statement_id = @BankStatementId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.OpenAsync();
                    using (var popoTransaction = await popoupaDBConnection.BeginTransactionAsync())
                    {
                        try
                        {
                            var expensesIds = (await popoupaDBConnection.QueryAsync<Guid?>(sqlOrder.ToString(), new
                            {
                                BankStatementId = bankStatementId
                            })).ToArray();

                            var expenses = new List<Expense?>();
                            foreach (Guid? expenseId in expensesIds)
                            {
                                if (expenseId is not null)
                                {
                                    var expense = await Get(expenseId.Value, userId);
                                    expenses.Add(expense);
                                }
                            }
                            await popoTransaction.CommitAsync();
                            await popoupaDBConnection.CloseAsync();
                            return expenses;
                        }
                        catch (Exception exception)
                        {
                            popoTransaction.Rollback();
                            throw new Exception($"Error in the PostgresDB! Error during multiple expenses query! The transaction was rolledBack! Error: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during multiple expenses query: {exception.Message}");
                }
            }
        }

        public async Task<Expense[]> GetAll(Filter filters, Guid userId)
        {
            var dynamicParams = new DynamicParameters();

            var sqlOrder = new StringBuilder();

            sqlOrder.Append
                (
                    @"
                    SELECT
                        expenses.expense_id AS Id,
                        expenses.description AS Description,
                        expenses.amount AS Amount,
                        expenses.date AS Date,
                        expenses.expense_category AS Category,
                        expenses.expense_group AS Group,
                        expenses.owner_id AS OwnerId,
                        expenses.is_shared AS IsShared
                    FROM
                        expenses
                    INNER JOIN
                        user_expenses_shares
                    ON
                        expenses.expense_id = user_expenses_shares.expense_id
                    WHERE 
                        user_expenses_shares.user_id = @UserId"
                );

            dynamicParams.Add("UserId", userId);
            if (filters.ExpenseDescription is not null)
            {
                sqlOrder.Append
                (
                    @"
                        AND description ILIKE @Description
                        "
                );
                dynamicParams.Add("Description", filters.ExpenseDescription);
            }
            if (filters.StartingDate is not null)
            {
                sqlOrder.Append
                (
                    @"
                    AND date >= @StartingDate
                    "
                );
                dynamicParams.Add("StartingDate", filters.StartingDate);
            }
            if (filters.EndingDate is not null)
            {
                sqlOrder.Append
                (
                    @"
                    AND date <= @EndingDate
                    "
                );
                dynamicParams.Add("EndingDate", filters.EndingDate);
            }
            if (filters.CategoryId is not null)
            {
                sqlOrder.Append
                (
                    @"
                    AND expense_category = @CategoryId
                    "
                );
                dynamicParams.Add("CategoryId", filters.CategoryId);
            }
            if (filters.GroupId is not null)
            {
                sqlOrder.Append
                (
                    @"
                    AND expense_group = @GroupId
                    "
                );
                dynamicParams.Add("GroupId", filters.GroupId);
            }
            sqlOrder.Append(';');

            const string categoriesSQLOrder =
            @"
                SELECT
                category_id AS Id,
                name AS Name
                FROM
                categories
                WHERE
                category_id = ANY(@CategoriesIds);
            ";

            const string groupsSQLOrder =
            @"
                SELECT
                group_id AS Id,
                name AS Name
                FROM
                groups
                WHERE
                group_id = ANY(@GroupsIds);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.OpenAsync();
                    using (var popoTransaction = await popoupaDBConnection.BeginTransactionAsync())
                    {
                        try
                        {
                            var expenseEntities = (await popoupaDBConnection.QueryAsync<ExpenseEntity>(sqlOrder.ToString(), dynamicParams)).ToArray();

                            var categoryMap = new Dictionary<Guid, Category?>();
                            var groupMap = new Dictionary<Guid, Group?>();
                            

                            var expenses = new Expense[expenseEntities.Length];

                            

                            for (int index = 0; index < expenseEntities.Length; index++)
                            {
                                var expenseEntity = expenseEntities[index];
                                if (expenseEntity is not null)
                                {
                                    if (!categoryMap.TryGetValue(expenseEntity.ExpenseCategoryId, out _))
                                    {
                                        categoryMap.Add(expenseEntity.ExpenseCategoryId, null);
                                    }
                                    if (!groupMap.TryGetValue(expenseEntity.ExpenseGroupId, out _))
                                    {
                                        groupMap.Add(expenseEntity.ExpenseGroupId, null);
                                    }
                                }
                            }
                            
                            var categoriesIds = categoryMap.Keys.ToList();
                            var groupsIds = groupMap.Keys.ToList();

                            var categories = (await popoupaDBConnection.QueryAsync<Category>(categoriesSQLOrder, new
                            {
                                CategoriesIds = categoriesIds
                            })).ToArray();
                            var groups = (await popoupaDBConnection.QueryAsync<Group>(groupsSQLOrder, new
                            {
                                GroupsIds = groupsIds
                            })).ToArray();



                            foreach (Category category in categories)
                            {
                                categoryMap[category.Id] = category;
                            }
                            foreach (Group group in groups)
                            {
                                groupMap[group.Id] = group;
                            }

                            for (int index = 0; index < expenseEntities.Length; index++)
                            {
                                var expenseEntity = expenseEntities[index];
                                var category = categoryMap[expenseEntity.ExpenseCategoryId] ?? throw new Exception("Error during expense category query, the category is null!");
                                var group = groupMap[expenseEntity.ExpenseGroupId] ?? throw new Exception("Error during expense group query, the group is null!");

                                var expense = new Expense(expenseEntity.Id, expenseEntity.Description, expenseEntity.Amount, expenseEntity.Date, category, group, expenseEntity.IsShared , expenseEntity.OwnerId);
                                expenses[index] = expense;
                            }


                            await popoTransaction.CommitAsync();
                            await popoupaDBConnection.CloseAsync();
                            return expenses;
                        }
                        catch (Exception exception)
                        {
                            popoTransaction.Rollback();
                            throw new Exception($"Error in the PostgresDB! Error during multiple expenses query! The transaction was rolledBack! Error: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during multiple expenses query: {exception.Message}");
                }
            }
        }

        public async Task ShareExpense(List<(Guid, decimal)> toShareUsersIdsAndShares, Guid expenseId)
        {
            const string sqlOrder = @"
            INSERT INTO
            user_expenses_shares (user_id, expense_id, share)
            VALUES
            (@UserId, @ExpenseId, @Share);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                await popoupaDBConnection.OpenAsync();
                try
                {
                    using (var popoTransaction = await popoupaDBConnection.BeginTransactionAsync())
                    {
                        try
                        {
                            foreach ((Guid userId, decimal share) in toShareUsersIdsAndShares)
                            {
                                await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                                {
                                    UserId = userId,
                                    ExpenseId = expenseId,
                                    Share = share
                                });
                            }
                            await popoTransaction.CommitAsync();
                            await popoupaDBConnection.CloseAsync();
                        }
                        catch (Exception exception)
                        {
                            popoTransaction.Rollback();
                            throw new Exception($"Error in the PostgresDB! Error during expense share query! The transaction was rolledBack! Error: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during expense share deletion query: {exception.Message}");
                }
            }
        }

        public async Task Update(Expense expense)
        {
            const string sqlOrder = @"
                UPDATE
                expenses
                SET
                expense_id = @ExpenseId,
                description = @Description,
                amount = @Amount,
                date = @Date,
                expense_category = @CategoryId,
                expense_group = @GroupId,
                owner_id = @OwnerId,
                is_shared = @IsShared
                WHERE 
                expense_id = @ExpenseId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        ExpenseId = expense.Id,
                        Description = expense.Description,
                        Amount = expense.Amount,
                        Date = expense.Date,
                        CategoryId = expense.Category.Id,
                        GroupId = expense.Group.Id,
                        OwnerId = expense.OwnerId,
                        IsShared = expense.IsShared
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during expense update query: {exception.Message}");
                }
            }
        }

        public async Task UpdateMultiple(List<Expense> expenses)
        {
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                await popoupaDBConnection.OpenAsync();
                try
                {
                    using (var popoTransaction = await popoupaDBConnection.BeginTransactionAsync())
                    {
                        try
                        {
                            foreach (Expense expense in expenses)
                            {
                                await Update(expense);
                            }
                            await popoTransaction.CommitAsync();
                            await popoupaDBConnection.CloseAsync();
                        }
                        catch (Exception exception)
                        {
                            popoTransaction.Rollback();
                            throw new Exception($"Error in the PostgresDB! Error during expenses update query! The transaction was rolledBack! Error: {exception.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during multiples expenses updates query: {exception.Message}");
                }
            }
        }
    }
}
