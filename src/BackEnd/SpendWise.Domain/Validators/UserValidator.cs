using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Validators.Contracts;

namespace SpendWise.Domain.Validators;

public class UserValidator : IUserValidator
{
    public ValidationErrors Validate(User user)
    {
        return new ValidationErrors();
    }
}