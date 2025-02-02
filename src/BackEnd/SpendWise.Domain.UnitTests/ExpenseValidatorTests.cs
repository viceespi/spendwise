using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpendWise.Domain.Models.ExpenseModels;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Validators;
using Xunit;

namespace SpendWise.Domain.UnitTests
{
    public class ExpenseValidatorTests
    {
        [Fact]
        public void ValidateExpense_InputIsValidExpense_ReturnsNoErrors()
        {
            // Arrange

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.Empty;
            Expense expense = new(description, date, amount, id);
            ExpenseValidator validator = new();

            // Act 

            ValidationErrors validatorErrors = validator.Validate(expense);

            // Assert

            Assert.False(validatorErrors.HasError);
            Assert.Empty(validatorErrors.Errors);

        }

        [Fact]
        public void ValidateExpense_EmptyDescription_ReturnsEmptyDescriptionError()
        {
            // Arrange

            string description = string.Empty;
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.Empty;
            Expense expense = new(description, date, amount, id);
            ExpenseValidator validator = new();

            // Act 

            ValidationErrors validatorErrors = validator.Validate(expense);

            // Assert

            Assert.True(validatorErrors.HasError);
            Assert.Contains("The description is empty!", validatorErrors.Errors);

        }

        [Fact]
        public void ValidateExpense_LongDescription_ReturnsLongDescriptionError()
        {
            // Arrange

            string description = new string('m', 251);
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.Empty;
            Expense expense = new(description, date, amount, id);
            ExpenseValidator validator = new();

            // Act 

            ValidationErrors validatorErrors = validator.Validate(expense);

            // Assert

            Assert.True(validatorErrors.HasError);
            Assert.Contains("Expense description is invalid! It must have less than 250 characters!", validatorErrors.Errors);

        }

        [Fact]
        public void ValidateExpense_ShortDescription_ReturnsShortDescriptionError()
        {
            // Arrange

            string description = "N";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 20;
            Guid id = Guid.Empty;
            Expense expense = new(description, date, amount, id);
            ExpenseValidator validator = new();

            // Act 

            ValidationErrors validatorErrors = validator.Validate(expense);

            // Assert

            Assert.True(validatorErrors.HasError);
            Assert.Contains("Expense description is invalid! It must have more than 1 character!", validatorErrors.Errors);

        }

        [Fact]
        public void ValidateExpense_FutureDate_ReturnsFutureDateError()
        {
            // Arrange

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(3000, 9, 29);
            decimal amount = 20;
            Guid id = Guid.Empty;
            Expense expense = new(description, date, amount, id);
            ExpenseValidator validator = new();

            // Act 

            ValidationErrors validatorErrors = validator.Validate(expense);

            // Assert

            Assert.True(validatorErrors.HasError);
            Assert.Contains("Expense date is invalid!", validatorErrors.Errors);

        }

        [Fact]
        public void ValidateExpense_AmountLessOrEqualToZero_ReturnsAmountLessOrEqualToZeroError()
        {
            // Arrange

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 0;
            Guid id = Guid.Empty;
            Expense expense = new(description, date, amount, id);
            ExpenseValidator validator = new();

            // Act 

            ValidationErrors validatorErrors = validator.Validate(expense);

            // Assert

            Assert.True(validatorErrors.HasError);
            Assert.Contains("Expense amount is invalid! It must be more than 0 BRL!", validatorErrors.Errors);

        }

        [Fact]
        public void ValidateExpense_AmountExceedingTenMillion_ReturnsAmountExceedingTenMillionError()
        {
            // Arrange

            string description = "NenegaCalamitosa";
            DateTime date = new DateTime(2024, 9, 29);
            decimal amount = 10000000001;
            Guid id = Guid.Empty;
            Expense expense = new(description, date, amount, id);
            ExpenseValidator validator = new();

            // Act 

            ValidationErrors validatorErrors = validator.Validate(expense);

            // Assert

            Assert.True(validatorErrors.HasError);
            Assert.Contains("Expense amount is invalid! It must be less than 10.000.000.000 BRL!", validatorErrors.Errors);

        }
    }
}