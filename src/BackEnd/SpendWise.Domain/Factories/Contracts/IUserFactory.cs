using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;

namespace SpendWise.Domain.Factories.Contracts;

public interface IUserFactory
{
    Result<User> CreateUserFromUserDto(UserDto userDto);
}