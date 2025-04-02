using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;

namespace SpendWise.Domain.Validators.Contracts;

public interface IUserValidator
{
    public ValidationErrors Validate(User user);
}