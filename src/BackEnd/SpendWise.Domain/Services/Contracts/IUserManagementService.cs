using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;

namespace SpendWise.Domain.Services.Contracts;

public interface IUserManagementService
{
    Task<Result<User>> CreateUser(UserDto user);
    
    Task<List<User>> GetAllUsers();
    
    Task<Result<User>> GetUserById(Guid id);
    
    Task DeleteUser(Guid id);
    
    Task<Result<User>> UpdateUser(UserDto user);
    
    Task<List<User>> GetFriends(Guid userId);
}