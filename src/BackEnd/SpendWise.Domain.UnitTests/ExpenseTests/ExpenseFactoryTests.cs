using NSubstitute;
using SpendWise.Domain.Factories;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Validators.Contracts;

namespace SpendWise.Domain.UnitTests.ExpenseTests
{
    public class ExpenseFactoryTests
    {
        private readonly IExpenseValidator _expenseValidator = Substitute.For<IExpenseValidator>();

        [Fact]
        public void CreateExpenseFromNewExpenseDTO_InputIsValidNewExpenseDTO_ReturnsCreatedExpense()
        {
            // Arrange

            ValidationErrors errors = new();
            ExpenseFactory factory = new(_expenseValidator);
            _expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid ownerId = Guid.Empty;

            NewExpenseDto newExpenseDto = new(description, date, amount, ownerId);
            Expense expectedExpense = new(description, date, amount, Guid.Empty, ownerId);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromNewExpenseDto(newExpenseDto);

            // Assert

            Assert.True(factoryResult.ValidationErrors is null);
            Assert.Equivalent(expectedExpense, factoryResult.OperationResult);
        }

        [Fact]
        public void CreateExpenseFromNewExpenseDTO_InputIsInvalidNewExpenseDTO_ReturnsAnyErrors()
        {
            // Arrange

            ValidationErrors errors = new();
            errors.Errors.Add("Tem erro");
            ExpenseFactory factory = new(_expenseValidator);
            _expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid ownerId = Guid.Empty;

            NewExpenseDto newExpenseDto = new(description, date, amount, ownerId);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromNewExpenseDto(newExpenseDto);

            // Assert

            Assert.NotNull(factoryResult.ValidationErrors);
        }

        [Fact]
        public void CreateExpenseFromToUpdateExpenseDTO_InputIsValidToUpdateExpenseDTO_ReturnsCreatedExpense()
        {
            // Arrange

            ValidationErrors errors = new();
            ExpenseFactory factory = new(_expenseValidator);
            _expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;

            ToUpdateExpenseDto toUpdateExpenseDto = new(description, date, amount, id, ownerId);
            Expense expectedExpense = new(description, date, amount, Guid.Empty, ownerId);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromToUpdateExpenseDto(toUpdateExpenseDto);

            // Assert

            Assert.True(factoryResult.ValidationErrors is null);
            Assert.Equivalent(expectedExpense, factoryResult.OperationResult);
        }

        [Fact]
        public void CreateExpenseFromToUpdateExpenseDTO_InputIsInvalidToUpdateExpenseDTO_ReturnsAnyErrors()
        {
            // Arrange

            ValidationErrors errors = new();
            errors.Errors.Add("Tem erro");
            ExpenseFactory factory = new(_expenseValidator);
            _expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;

            ToUpdateExpenseDto toUpdateExpenseDto = new(description, date, amount, id, ownerId);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromToUpdateExpenseDto(toUpdateExpenseDto);

            // Assert

            Assert.NotNull(factoryResult.ValidationErrors);
        }
    }
}