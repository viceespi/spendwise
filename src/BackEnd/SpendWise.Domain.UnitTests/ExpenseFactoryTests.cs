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
        public void ValidateExpense_InputIsValidExpense_ReturnsNoErrors()
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
            Assert.Equivalent(expectedExpense,factoryResult.OperationResult);
        
        }

        [Fact]
        public void ValidateExpense_InputIsInvalid_ReturnsAnyErrors()
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
    }
}