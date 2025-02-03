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
        private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();

        private readonly IExpenseFactory expenseFactory = Substitute.For<IExpenseFactory>();

        [Fact]
        public async Task CreateNewExpense_InputIsValid_ReturningCreatedExpenseGuid()
        {

            // Arrange

            ExpenseManagementServices expenseServices = new(expenseFactory, expenseRepository);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();

            NewExpenseDTO expenseDTO = new(description, date, amount);

            Expense expectedExpense = new(description, date, amount, id);
            Result<Expense> factoryInputResult = new(expectedExpense);

            expenseFactory.CreateExpenseFromNewExpenseDTO(Arg.Any<NewExpenseDTO>()).Returns(factoryInputResult);
            expenseRepository.CreateNewExpense(Arg.Any<Expense>()).Returns(id);

            // Act

            Result<Guid> serviceResult = await expenseServices.CreateExpense(expenseDTO);

            // Assert

            Assert.True(serviceResult.ValidationErrors is null);
            Assert.Equal(id, serviceResult.OperationResult);

        }

        [Fact]
        public async Task ValidateExpense_InputIsInvalid_ReturnsAnyErrors()
        {
            // Arrange

            ExpenseManagementServices expenseServices = new(expenseFactory, expenseRepository);
            ValidationErrors errors = new();
            errors.Errors.Add("There are errors");
            Result<Expense> factoryInputResult = new(errors);


            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;

            NewExpenseDTO newExpenseDTO = new(description, date, amount);

            expenseFactory.CreateExpenseFromNewExpenseDTO(Arg.Any<NewExpenseDTO>()).Returns(factoryInputResult);

            // Act 

            Result<Guid> factoryResult = await expenseServices.CreateExpense(newExpenseDTO);

            // Assert

            Assert.NotNull(factoryResult.ValidationErrors);

        }

        [Fact]
        public async Task GetExpense_ExpenseExists_ReturnExpense()
        {

            // Arrange
            ExpenseManagementServices expenseServices = new(expenseFactory, expenseRepository);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();

            Expense expectedExpense = new(description, date, amount, id);

            expenseRepository.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(expectedExpense));

            // Act

            Expense? testExpense = await expenseServices.GetExpense(id);

            // Assert

            Assert.Equivalent(expectedExpense, testExpense);

        }

        [Fact]
        public async Task GetExpense_ExpenseDoNotExists_ReturnNull()
        {

            // Arrange
            ExpenseManagementServices expenseServices = new(expenseFactory, expenseRepository);
            Guid id = Guid.NewGuid();

            expenseRepository.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(null));

            // Act

            Expense? testExpense = await expenseServices.GetExpense(id);

            // Assert

            Assert.Null(testExpense);

        }

        [Fact]
        public async Task GetAllExpenses_ExpensesExists_ReturnListNotEmpty()
        {

            // Arrange
            List<Expense> expectedExpenses = new()
            {
                new Expense("NenegaCalamitosa", new DateTime(2024, 9, 29), 20m, Guid.NewGuid()),
                new Expense("Café da Manhã", new DateTime(2024, 9, 30), 15.50m, Guid.NewGuid()),
                new Expense("Almoço", new DateTime(2024, 10, 1), 32.90m, Guid.NewGuid()),
                new Expense("Supermercado", new DateTime(2024, 10, 2), 120.75m, Guid.NewGuid()),
                new Expense("Transporte", new DateTime(2024, 10, 3), 8.40m, Guid.NewGuid()),
                new Expense("Cinema", new DateTime(2024, 10, 4), 45.00m, Guid.NewGuid()),
                new Expense("Conta de Luz", new DateTime(2024, 10, 5), 210.30m, Guid.NewGuid()),
                new Expense("Internet", new DateTime(2024, 10, 6), 89.99m, Guid.NewGuid()),
                new Expense("Farmácia", new DateTime(2024, 10, 7), 56.20m, Guid.NewGuid()),
                new Expense("Academia", new DateTime(2024, 10, 8), 99.90m, Guid.NewGuid())
            };

            ExpenseManagementServices expenseServices = new(expenseFactory, expenseRepository);
            expenseRepository.GetAllExpenses().Returns(Task.FromResult(expectedExpenses));

            // Act

            List<Expense> testExpenses = await expenseServices.GetAllExpenses();

            // Assert

            Assert.Equivalent(expectedExpenses, testExpenses);

        }

        [Fact]
        public async Task GetAllExpenses_ExpensesDoNotExist_ReturnListEmpty()
        {

            // Arrange
            List<Expense> expectedExpenses = new();
            ExpenseManagementServices expenseServices = new(expenseFactory, expenseRepository);
            expenseRepository.GetAllExpenses().Returns(Task.FromResult(expectedExpenses));

            // Act

            List<Expense> testExpenses = await expenseServices.GetAllExpenses();

            // Assert

            Assert.Empty(testExpenses);

        }
    }
}