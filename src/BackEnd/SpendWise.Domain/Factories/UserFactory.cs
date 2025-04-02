using SpendWise.Domain.Factories.Contracts;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Validators.Contracts;

namespace SpendWise.Domain.Factories;

public class UserFactory : IUserFactory
{
    private IUserValidator _validator;

    public UserFactory(IUserValidator validator)
    {
        this._validator = validator;
    }
    
    public Result<User> CreateUserFromUserDto(UserDto userDto)
    {
        User newUser = new(userDto.Name, Guid.Empty);
        ValidationErrors userValidation = _validator.Validate(newUser);
        if (userValidation.HasError)
        {
            Result<User> failedResult = new(userValidation);
            return failedResult;
        }
        Result<User> successResult = new(newUser);
        return successResult;
    }
}