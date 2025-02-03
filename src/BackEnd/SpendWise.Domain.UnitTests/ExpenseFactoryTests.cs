using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using SpendWise.Domain.Factories;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Validators.Contracts;
using Xunit;

namespace SpendWise.Domain.UnitTests
{
    public class ExpenseFactoryTests
    {
        private readonly IExpenseValidator expenseValidator = Substitute.For<IExpenseValidator>();

        [Fact]
        public void CreateExpenseFromNewExpenseDTO_InputIsValidNewExpenseDTO_ReturnsCreatedExpense()
        {
            // Arrange

            ValidationErrors errors = new();
            ExpenseFactory factory = new(expenseValidator);
            expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;

            NewExpenseDTO newExpenseDTO = new(description, date, amount);
            Expense expectedExpense = new(description, date, amount, Guid.Empty);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromNewExpenseDTO(newExpenseDTO);

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
            ExpenseFactory factory = new(expenseValidator);
            expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;

            NewExpenseDTO newExpenseDTO = new(description, date, amount);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromNewExpenseDTO(newExpenseDTO);

            // Assert

            Assert.NotNull(factoryResult.ValidationErrors);

        }

        [Fact]
        public void CreateExpenseFromToUpdateExpenseDTO_InputIsValidToUpdateExpenseDTO_ReturnsCreatedExpense()
        {
            // Arrange

            ValidationErrors errors = new();
            ExpenseFactory factory = new(expenseValidator);
            expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();

            ToUpdateExpenseDTO toUpdateExpenseDTO = new(description, date, amount, id);
            Expense expectedExpense = new(description, date, amount, Guid.Empty);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromToUpdateExpenseDTO(toUpdateExpenseDTO);

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
            ExpenseFactory factory = new(expenseValidator);
            expenseValidator.Validate(Arg.Any<Expense>()).Returns(errors);

            string description = "";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.NewGuid();

            ToUpdateExpenseDTO toUpdateExpenseDTO = new(description, date, amount, id);

            // Act 

            Result<Expense> factoryResult = factory.CreateExpenseFromToUpdateExpenseDTO(toUpdateExpenseDTO);

            // Assert

            Assert.NotNull(factoryResult.ValidationErrors);

        }
    }
}