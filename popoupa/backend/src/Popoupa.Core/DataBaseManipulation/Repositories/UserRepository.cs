using Dapper;
using Npgsql;
using Popoupa.Core.DataBaseManipulation.Interfaces;
using Popoupa.Core.DataBaseManipulation.UserSecurity;
using Popoupa.Core.DomainModels;

namespace Popoupa.Core.DataBaseManipulation.Repositories
{

    public class UserRepository : IUserRepositoryInterface
    {
        private readonly string popoupaDB = "Server=192.168.0.21;Port=5432;Database=popoupa;User Id=postgres;Password=Nina100%";

        public async Task UserPipeLine()
        {
            const string sqlOrder = @"
                INSERT
                INTO
                users (user_id, name, email, password)
                VALUES
                ('3376322c-4657-4a3c-b4fc-3b713e2ba894', 'Angus', 'Angus@barkmail.com', 'AnguzinhoFofinho100%')
                ON CONFLICT (user_id) 
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
                    throw new Exception($"Error in PostgresDB! Error during user pipeline: {exception.Message}");
                }
            }
        }

        public async Task<Guid> Add(User newUser)
        {
            var passwordHasher = new PasswordHasher();
            var hashedPassword = passwordHasher.HashPassword(newUser.Password);
            Guid userGuid;
            const string sqlOrder = @"
                INSERT 
                INTO 
                users (name, email, password) 
                VALUES 
                (@UserName, @UserEmail, @UserPassword)
                RETURNING user_id;";
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var userId = await popoupaDBConnection.QuerySingleAsync<Guid>(sqlOrder, new
                    {
                        UserName = newUser.Name,
                        UserEmail = newUser.Email,
                        UserPassword = hashedPassword
                    });
                    userGuid = userId;

                    return userGuid;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in PostgresDB! Error during user creation: {exception.Message}");
                }
            }
        }
        public async Task Delete(Guid userId)
        {
            const string sqlOrder = @"
            DELETE
            FROM
            users
            WHERE
            user_id = @UserId;
            ";
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        UserId = userId
                    });

                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in PostgresDB! Error during user deletion: {exception.Message}");
                }
            }
        }

        public async Task<User?> Get(Guid userId)
        {
            const string sqlOrder = @"
            SELECT
            user_id as Id,
            name as Name,
            email as Email
            FROM
            users
            WHERE
            user_id = @UserId;
            ";
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var user = await popoupaDBConnection.QueryFirstOrDefaultAsync<User>(sqlOrder, new
                    {
                        UserGuid = userId
                    });

                    return user;

                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in PostgresDB! Error during user query: {exception.Message}");
                }
            }
        }

        public async Task<IEnumerable<User?>> GetAll(Guid userId)
        {
            const string sqlOrder = @"
                SELECT
                users.user_id as Id,
                users.name as Name,
                users.email as Email
                FROM
                users
                INNER JOIN 
                user_friends
                ON
                users.user_id = user_friends.friend_id
                WHERE
                user_friends.user_id = @UserId;
            ";
            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    var users = await popoupaDBConnection.QueryAsync<User>(sqlOrder, new
                    {
                        UserId = userId
                    });

                    return users;
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in PostgresDB! Error during user friends query: {exception.Message}");
                }
            }
        }

        public async Task Update(User user)
        {
            const string sqlOrder = @"
                UPDATE
                users
                SET
                user_id = @UserId,
                name = @UserName,
                email = @UserEmail,
                password = @UserPassword
                WHERE
                user_id = @UserId;
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        UserId = user.Id,
                        UserName = user.Name,
                        UserEmail = user.Email,
                        UserPassword = user.Password
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in PostgresDB! Error during user update query: {exception.Message}");
                }
            }
        }

        public async Task CreateUserFriendRelation(Guid userId, Guid friendId)
        {
            const string sqlOrder = @"
                INSERT INTO
                user_friends (user_id, friend_id)
                VALUES
                (@UserId, @FriendId);
            ";

            using (var popoupaDBConnection = new NpgsqlConnection(popoupaDB))
            {
                try
                {
                    await popoupaDBConnection.ExecuteAsync(sqlOrder, new
                    {
                        UserId = userId,
                        FriendId = friendId
                    });
                }
                catch (Exception exception)
                {
                    throw new Exception($"Error in PostgresDB! Error during user friendship creation query: {exception.Message}");
                }
            }
        }

    }
}