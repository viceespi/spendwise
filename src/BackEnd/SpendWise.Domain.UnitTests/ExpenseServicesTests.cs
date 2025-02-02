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
            Assert.Equal(id,serviceResult.OperationResult);

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
    }
}