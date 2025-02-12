using System.ComponentModel;
using Dapper;
using Npgsql;
using Popoupa.Core.DataBaseManipulation.Interfaces;
using Popoupa.Core.DomainModels;


namespace Popoupa.Core.DataBaseManipulation.Repositories
{
    public class CategoryRepository : ICategoryRepositoryInterface
    {
        private readonly string popoupaDB = "Server=192.168.0.21;Port=5432;Database=popoupa;User Id=postgres;Password=Nina100%";

        public async Task CategoryPipeLine()
        {
            const string sqlOrder =
            @"
            INSERT INTO categories (category_id, name) 
            VALUES ('00000000-0000-0000-0000-000000000000', 'Unassigned')
            ON CONFLICT (category_id) 
            DO NOTHING;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                   await popoupaDBConnection.ExecuteAsync(sqlOrder);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during category pipeline query: {exception.Message}");
                }
            }
        }
        public async Task<Guid> Add(Category newCategory)
        {
            const string sqlOrder =
            @"
            INSERT INTO
            categories (name)
            VALUES
            (@CategoryName)
            RETURNING category_id;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var categoryId = await popoupaDBConnection.QuerySingleAsync<Guid>(sqlOrder, new
                    {
                        CategoryName = newCategory.Name
                    });

                    return categoryId;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during category creation query: {exception.Message}");
                }
            }
        }

        public async Task Delete(Guid categoryId)
        {
            const string sqlOrder = @"
                DELETE
                FROM
                categories
                WHERE
                category_id = @CategoryId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        CategoryId = categoryId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during category deletion query: {exception.Message}");
                }
            }
        }

        public async Task<Category?> Get(Guid categoryId)
        {
            const string sqlOrder = @"
                SELECT
                categories.category_id as Id,
                categories.name as Name
                FROM
                categories
                WHERE
                category_id = @CategoryId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var category = await popoupaDBConnection.QuerySingleAsync<Category?>(sqlOrder, new
                    {
                        CategoryId = categoryId
                    });

                    return category;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during category query: {exception.Message}");
                }
            }
        }

        public async Task<List<Category?>> GetAll(Guid userId)
        {
            const string sqlOrder = @"
                SELECT
                categories.category_id as Id,
                categories.name as Name
                FROM
                categories
                INNER JOIN
                user_categories
                ON
                categories.category_id = user_categories.category_id
                WHERE
                user_categories.user_id = @UserId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var categories = await popoupaDBConnection.QueryAsync<Category?>(sqlOrder, new
                    {
                        UserId = userId
                    });

                    return categories.ToList();
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during categories query: {exception.Message}");
                }
            }
        }

        public async Task Update(Category category)
        {
            const string sqlOrder = @"
                UPDATE
                categories
                SET
                category_id = @CategoryId,
                name = @CategoryName
                WHERE
                category_id = @CategoryId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        categoryId = category.Id,
                        categoryName = category.Name
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during category update query: {exception.Message}");
                }
            }
        }

        public async Task CreateUserCategoryRelation(Guid userId, Guid categoryId)
        {
            const string sqlOrder = @"
                INSERT INTO
                user_categories(user_id, category_id)
                VALUES
                (@UserId, @CategoryId);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        UserId = userId,
                        CategoryId = categoryId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in the PostgresDB! Error during user category relation creation query: {exception.Message}");
                }
            }
        }
    }
}
