using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Repositories.Contracts;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace SpendWise.Domain.Repositories;

[ExcludeFromCodeCoverage(Justification = "The IDbConnection query funcions are extention methods, and can't be mocked.")]

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;

    public UserRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public Task<User> CreateUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetUsers()
    {
        const string sqlOrder =
            @"
            SELECT
            name AS Name,
            user_id AS Id
            FROM
            users;
            ";
        
        List<User> users = (await _connection.QueryAsync<User>(sqlOrder)).ToList();
        return users;
    }

    public async Task<List<User>> GetFriends(Guid userId)
    {
        const string sqlOrder =
            @"
            SELECT
            users.name AS Name,
            users.user_id AS Id
            FROM
            users
           INNER JOIN
            user_friends on users.user_id = user_friends.friend_id
            WHERE
            user_friends.user_id = @UserId;
            ";
        
        List<User> users = (await _connection.QueryAsync<User>(sqlOrder, new
        {
            UserId = userId
        })).ToList();
        return users;
    }
}