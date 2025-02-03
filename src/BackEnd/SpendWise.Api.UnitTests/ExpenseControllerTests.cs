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
    public class ExpenseControllerTests
    {
        private readonly IExpenseManagementServices expenseServices = Substitute.For<IExpenseManagementServices>();

        [Fact]
        public async Task CreateNewExpense_InputIsValid_ReturningOk()
        {

            // Arrange
            ExpensesController expenseController = new(expenseServices);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid expectedId = Guid.NewGuid();
            Result<Guid> expectedResult = new(expectedId);

            NewExpenseDTO expenseDTO = new(description, date, amount);

            expenseServices.CreateExpense(Arg.Any<NewExpenseDTO>()).Returns(Task.FromResult(expectedResult));

            // Act

            IActionResult controllerResult = await expenseController.CreateExpense(expenseDTO);

            // Assert

            Assert.IsType<OkObjectResult>(controllerResult);
        }

        [Fact]
        public async Task CreateNewExpense_InputIsInvalid_ReturningBadRequest()
        {

            // Arrange
            ExpensesController expenseController = new(expenseServices);

            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            ValidationErrors errors = new();
            errors.Errors.Add("Has error");
            Result<Guid> expectedResult = new(errors);

            NewExpenseDTO expenseDTO = new(description, date, amount);

            expenseServices.CreateExpense(Arg.Any<NewExpenseDTO>()).Returns(Task.FromResult(expectedResult));

            // Act

            IActionResult controllerResult = await expenseController.CreateExpense(expenseDTO);

            // Assert

            Assert.IsType<BadRequestObjectResult>(controllerResult);
        }

        [Fact]
        public async Task GetExpense_ExpenseExists_ReturningOk()
        {

            // Arrange
            ExpensesController expenseController = new(expenseServices);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();

            Expense expectedExpense = new(description, date, amount, id);

            expenseServices.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(expectedExpense));

            // Act

            IActionResult controllerResult = await expenseController.GetExpense(id);

            // Assert

            Assert.IsType<OkObjectResult>(controllerResult);
        }

        [Fact]
        public async Task GetExpense_ExpenseDoNotExists_ReturningNotFound()
        {

            // Arrange
            ExpensesController expenseController = new(expenseServices);

            Guid id = Guid.NewGuid();

            expenseServices.GetExpense(Arg.Any<Guid>()).Returns(Task.FromResult<Expense?>(null));

            // Act

            IActionResult controllerResult = await expenseController.GetExpense(id);

            // Assert

            Assert.IsType<NotFoundResult>(controllerResult);
        }

        [Fact]
        public async Task GetAllExpense_ExpenseExists_ReturningOk()
        {

            // Arrange
            ExpensesController expenseController = new(expenseServices);

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

            expenseServices.GetAllExpenses().Returns(Task.FromResult(expectedExpenses));

            // Act

            IActionResult controllerResult = await expenseController.GetAllExpenses();

            // Assert

            Assert.IsType<OkObjectResult>(controllerResult);
        }
    }
}