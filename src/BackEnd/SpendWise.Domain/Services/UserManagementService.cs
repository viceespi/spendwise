using SpendWise.Domain.Factories.Contracts;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Repositories.Contracts;
using SpendWise.Domain.Services.Contracts;

namespace SpendWise.Domain.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _repository;
    
    private readonly IUserFactory  _factory;

    public UserManagementService(IUserRepository repository, IUserFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }
    
    public Task<Result<User>> CreateUser(UserDto user)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetAllUsers()
    {
        List<User> users = await _repository.GetUsers();
        return users;
    }

    public Task<Result<User>> GetUserById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> UpdateUser(UserDto user)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetFriends(Guid userId)
    {
        List<User> friends = await _repository.GetFriends(userId);
        return friends;
    }
}