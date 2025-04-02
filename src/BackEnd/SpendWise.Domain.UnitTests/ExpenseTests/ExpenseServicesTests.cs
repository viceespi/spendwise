using System.Threading.Tasks;
using NSubstitute;
using SpendWise.Domain.Factories.Contracts;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Repositories.Contracts;
using SpendWise.Domain.Services;

namespace SpendWise.Domain.UnitTests
{
    public class ExpenseServicesTests
    {
        private readonly IExpenseRepository _expenseRepository = Substitute.For<IExpenseRepository>();

        private readonly IExpenseFactory _expenseFactory = Substitute.For<IExpenseFactory>();

        [Fact]
        public async Task CreateNewExpense_InputIsValid_ReturningCreatedExpenseGuid()
        {
            // Arrange

            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;

            NewExpenseDto expenseDto = new(description, date, amount, ownerId);

            Expense expectedExpense = new(description, date, amount, id, ownerId);
            Result<Expense> factoryInputResult = new(expectedExpense);

            _expenseFactory.CreateExpenseFromNewExpenseDto(Arg.Any<NewExpenseDto>()).Returns(factoryInputResult);
            _expenseRepository.CreateNewExpense(Arg.Any<Expense>()).Returns(expectedExpense);

            // Act

            Result<Expense> serviceResult = await expenseService.CreateExpense(expenseDto);

            // Assert

            Assert.True(serviceResult.ValidationErrors is null);
            Assert.Equal(expectedExpense, serviceResult.OperationResult);
        }

        [Fact]
        public async Task CreateNewExpense_InputIsInvalid_ReturnsAnyErrors()
        {
            // Arrange

            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);
            ValidationErrors errors = new();
            errors.Errors.Add("There are errors");
            Result<Expense> factoryInputResult = new(errors);


            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid ownerId = Guid.Empty;

            NewExpenseDto newExpenseDto = new(description, date, amount, ownerId);

            _expenseFactory.CreateExpenseFromNewExpenseDto(Arg.Any<NewExpenseDto>()).Returns(factoryInputResult);

            // Act 

            Result<Expense> factoryResult = await expenseService.CreateExpense(newExpenseDto);

            // Assert

            Assert.NotNull(factoryResult.ValidationErrors);
        }

        [Fact]
        public async Task GetExpense_ExpenseExists_ReturnExpense()
        {
            // Arrange
            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;

            Expense expectedExpense = new(description, date, amount, id, ownerId);

            _expenseRepository.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(expectedExpense));

            // Act

            Expense? testExpense = await expenseService.GetExpense(id);

            // Assert

            Assert.Equivalent(expectedExpense, testExpense);
        }

        [Fact]
        public async Task GetExpense_ExpenseDoNotExists_ReturnNull()
        {
            // Arrange
            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);
            Guid id = Guid.NewGuid();

            _expenseRepository.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(null));

            // Act

            Expense? testExpense = await expenseService.GetExpense(id);

            // Assert

            Assert.Null(testExpense);
        }

        [Fact]
        public async Task GetAllExpenses_ExpensesExists_ReturnListNotEmpty()
        {
            // Arrange
            Guid ownerId = Guid.Empty;
            List<Expense> expectedExpenses = new()
            {
                new Expense("NenegaCalamitosa", new DateTime(2024, 9, 29), 20m, Guid.NewGuid(), ownerId),
                new Expense("Café da Manhã", new DateTime(2024, 9, 30), 15.50m, Guid.NewGuid(), ownerId),
                new Expense("Almoço", new DateTime(2024, 10, 1), 32.90m, Guid.NewGuid(), ownerId),
                new Expense("Supermercado", new DateTime(2024, 10, 2), 120.75m, Guid.NewGuid(), ownerId),
                new Expense("Transporte", new DateTime(2024, 10, 3), 8.40m, Guid.NewGuid(), ownerId),
                new Expense("Cinema", new DateTime(2024, 10, 4), 45.00m, Guid.NewGuid(), ownerId),
                new Expense("Conta de Luz", new DateTime(2024, 10, 5), 210.30m, Guid.NewGuid(), ownerId),
                new Expense("Internet", new DateTime(2024, 10, 6), 89.99m, Guid.NewGuid(), ownerId),
                new Expense("Farmácia", new DateTime(2024, 10, 7), 56.20m, Guid.NewGuid(), ownerId),
                new Expense("Academia", new DateTime(2024, 10, 8), 99.90m, Guid.NewGuid(), ownerId)
            };

            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);
            _expenseRepository.GetAllExpenses(ownerId).Returns(Task.FromResult(expectedExpenses));

            // Act

            List<Expense> testExpenses = await expenseService.GetAllExpenses(ownerId);

            // Assert

            Assert.Equivalent(expectedExpenses, testExpenses);
        }

        [Fact]
        public async Task GetAllExpenses_ExpensesDoNotExist_ReturnListEmpty()
        {
            // Arrange
            Guid ownerId = Guid.Empty;
            List<Expense> expectedExpenses = new();
            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);
            _expenseRepository.GetAllExpenses(ownerId).Returns(Task.FromResult(expectedExpenses));

            // Act

            List<Expense> testExpenses = await expenseService.GetAllExpenses(ownerId);

            // Assert

            Assert.Empty(testExpenses);
        }

        [Fact]
        public async Task UpdateExpense_InputIsValid_ReturningUpdatedMessage()
        {
            // Arrange

            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;

            ToUpdateExpenseDto toUpdateExpenseDto = new(description, date, amount, id , ownerId);

            Expense factoryExpense = new(description, date, amount, id , ownerId);
            Expense updatedExpense = factoryExpense;
            Result<Expense> factoryInputResult = new(factoryExpense);

            _expenseFactory.CreateExpenseFromToUpdateExpenseDto(Arg.Any<ToUpdateExpenseDto>())
                .Returns(factoryInputResult);


            // Act

            Result<Expense> serviceResult = await expenseService.UpdateExpense(toUpdateExpenseDto);

            // Assert

            Assert.True(serviceResult.ValidationErrors is null);
            Assert.Equal(updatedExpense, serviceResult.OperationResult);
        }

        [Fact]
        public async Task UpdateExpense_InputIsInvalid_ReturnsAnyErrors()
        {
            // Arrange

            ExpenseManagementService expenseService = new(_expenseFactory, _expenseRepository);
            ValidationErrors errors = new();
            errors.Errors.Add("There are errors");
            Result<Expense> factoryInputResult = new(errors);


            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;


            ToUpdateExpenseDto toUpdateExpenseDto = new(description, date, amount, id,  ownerId);

            _expenseFactory.CreateExpenseFromToUpdateExpenseDto(Arg.Any<ToUpdateExpenseDto>())
                .Returns(factoryInputResult);

            // Act 

            Result<Expense> serviceResult = await expenseService.UpdateExpense(toUpdateExpenseDto);

            // Assert

            Assert.NotNull(serviceResult.ValidationErrors);
        }
    }
}