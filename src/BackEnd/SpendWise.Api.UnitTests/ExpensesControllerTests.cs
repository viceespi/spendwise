using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SpendWise.Api.Controllers;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Services.Contracts;
using Xunit;

namespace SpendWise.Api.UnitTests
{
    public class ExpensesControllerTests
    {
        private readonly IExpenseManagementService _expenseService = Substitute.For<IExpenseManagementService>();

        [Fact]
        public async Task CreateNewExpense_InputIsValid_ReturningOk()
        {

            // Arrange
            ExpensesController expenseController = new(_expenseService);

            const string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid ownerId = Guid.Empty;
            Guid expectedId = Guid.NewGuid();
            Expense expectedExpense = new Expense(description, date, amount, ownerId, expectedId);
            Result<Expense> expectedResult = new(expectedExpense);

            NewExpenseDto expenseDto = new(description, date, amount, ownerId);

            _expenseService.CreateExpense(Arg.Any<NewExpenseDto>()).Returns(Task.FromResult(expectedResult));

            // Act

            IActionResult controllerResult = await expenseController.CreateExpense(expenseDto);

            // Assert

            Assert.IsType<OkObjectResult>(controllerResult);
        }

        [Fact]
        public async Task CreateNewExpense_InputIsInvalid_ReturningBadRequest()
        {

            // Arrange
            ExpensesController expenseController = new(_expenseService);

            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid ownerId = Guid.Empty;
            ValidationErrors errors = new();
            errors.Errors.Add("Has error");
            Result<Expense> expectedResult = new(errors);

            NewExpenseDto expenseDto = new(description, date, amount, ownerId);

            _expenseService.CreateExpense(Arg.Any<NewExpenseDto>()).Returns(Task.FromResult(expectedResult));

            // Act

            IActionResult controllerResult = await expenseController.CreateExpense(expenseDto);

            // Assert

            Assert.IsType<BadRequestObjectResult>(controllerResult);
        }

        [Fact]
        public async Task GetExpense_ExpenseExists_ReturningOk()
        {

            // Arrange
            ExpensesController expenseController = new(_expenseService);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;

            Expense expectedExpense = new(description, date, amount, id, ownerId);

            _expenseService.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(expectedExpense));

            // Act

            IActionResult controllerResult = await expenseController.GetExpense(id);

            // Assert

            Assert.IsType<OkObjectResult>(controllerResult);
        }

        [Fact]
        public async Task GetExpense_ExpenseDoNotExists_ReturningNotFound()
        {

            // Arrange
            ExpensesController expenseController = new(_expenseService);

            Guid id = Guid.NewGuid();

            _expenseService.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(null));

            // Act

            IActionResult controllerResult = await expenseController.GetExpense(id);

            // Assert

            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task GetAllExpense_ExpenseExists_ReturningOk()
        {

            // Arrange
            ExpensesController expenseController = new(_expenseService);
            Guid ownerId = Guid.Empty;

            List<Expense> expectedExpenses = new()
            {
                new Expense("NenegaCalamitosa", new DateTime(2024, 9, 29), 20m, Guid.NewGuid(),  ownerId),
                new Expense("Café da Manhã", new DateTime(2024, 9, 30), 15.50m, Guid.NewGuid(),  ownerId ),
                new Expense("Almoço", new DateTime(2024, 10, 1), 32.90m, Guid.NewGuid(),  ownerId),
                new Expense("Supermercado", new DateTime(2024, 10, 2), 120.75m, Guid.NewGuid(), ownerId),
                new Expense("Transporte", new DateTime(2024, 10, 3), 8.40m, Guid.NewGuid(),  ownerId),
                new Expense("Cinema", new DateTime(2024, 10, 4), 45.00m, Guid.NewGuid(),  ownerId ),
                new Expense("Conta de Luz", new DateTime(2024, 10, 5), 210.30m, Guid.NewGuid(), ownerId),
                new Expense("Internet", new DateTime(2024, 10, 6), 89.99m, Guid.NewGuid(), ownerId),
                new Expense("Farmácia", new DateTime(2024, 10, 7), 56.20m, Guid.NewGuid(),  ownerId),
                new Expense("Academia", new DateTime(2024, 10, 8), 99.90m, Guid.NewGuid(), ownerId)
            };

            _expenseService.GetAllExpenses(ownerId).Returns(Task.FromResult(expectedExpenses));

            // Act

            IActionResult controllerResult = await expenseController.GetAllExpenses(ownerId);

            // Assert

            Assert.IsType<OkObjectResult>(controllerResult);
        }

        [Fact]
        public async Task UpdateExpense_InputIsValid_ReturningOk()
        {

            // Arrange
            ExpensesController expenseController = new(_expenseService);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();
            Guid ownerId = Guid.Empty;
            Expense updatedExpense = new(description, date, amount, ownerId, id);
            Result<Expense> expectedResult = new(updatedExpense);

            ToUpdateExpenseDto expenseDto = new(description, date, amount, id , ownerId);

            _expenseService.UpdateExpense(Arg.Any<ToUpdateExpenseDto>()).Returns(Task.FromResult(expectedResult));

            // Act

            IActionResult controllerResult = await expenseController.UpdateExpense(expenseDto);

            // Assert

            Assert.IsType<OkObjectResult>(controllerResult);
        }

        [Fact]
        public async Task UpdateExpense_InputIsInvalid_ReturningBadRequest()
        {

            // Arrange
            ExpensesController expenseController = new(_expenseService);

            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid ownerId = Guid.Empty;
            ValidationErrors errors = new();
            errors.Errors.Add("Has error");
            Result<Expense> expectedResult = new(errors);

            NewExpenseDto expenseDto = new(description, date, amount , ownerId);

            _expenseService.CreateExpense(Arg.Any<NewExpenseDto>()).Returns(Task.FromResult(expectedResult));

            // Act

            IActionResult controllerResult = await expenseController.CreateExpense(expenseDto);

            // Assert

            Assert.IsType<BadRequestObjectResult>(controllerResult);
        }
    }
}