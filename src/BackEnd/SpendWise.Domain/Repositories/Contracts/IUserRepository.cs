using SpendWise.Domain.Models.UserModels;

namespace SpendWise.Domain.Repositories.Contracts;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    
    Task<User> GetUser(Guid userId);
    
    Task<User> UpdateUser(User user);
    
    Task DeleteUser(Guid userId);
    
    Task<List<User>> GetUsers();

    Task<List<User>> GetFriends(Guid userId);
}