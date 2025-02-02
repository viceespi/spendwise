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
    }
}