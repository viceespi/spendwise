using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Validators.Contracts;

namespace SpendWise.Domain.Validators
{
    public class ExpenseValidator : IExpenseValidator
    {
        public ValidationErrors Validate(Expense expense)
        {
            ValidationErrors validationErrors = new();

            if (string.IsNullOrEmpty(expense.Description))
            {
                validationErrors.Errors.Add("The description is empty!");
            }
            else
            {
                if (expense.Description.Length > 250)
                {
                    validationErrors.Errors.Add("Expense description is invalid! It must have less than 250 characters!");
                }

                if (expense.Description.Length < 2)
                {
                    validationErrors.Errors.Add("Expense description is invalid! It must have more than 1 character!");
                }
            }

            // TODO => Make the time a external dependency, to avoid brute forcing my tests.
            if (expense.Date > DateTime.UtcNow)
            {
                validationErrors.Errors.Add("Expense date is invalid!");
            }

            if (expense.Amount <= 0)
            {
                validationErrors.Errors.Add("Expense amount is invalid! It must be more than 0 BRL!");
            }

            if (expense.Amount > 99999999.99m)
            {
                validationErrors.Errors.Add("Expense amount is invalid! It must be less than 10.000.000.000 BRL!");
            }

            return validationErrors;
        }
    }
}