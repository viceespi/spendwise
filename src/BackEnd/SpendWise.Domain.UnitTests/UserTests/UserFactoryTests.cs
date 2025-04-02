using NSubstitute;
using SpendWise.Domain.Factories;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Validators.Contracts;

namespace SpendWise.Domain.UnitTests.UserTests;

public class UserFactoryTests
{
    private readonly IUserValidator _userValidator = Substitute.For<IUserValidator>();

    [Fact]
    public void CreateUserFromUserDto_InputIsValid_ReturnsUser()
    {
        // Arrange

        ValidationErrors errors = new();
        UserFactory factory = new(_userValidator);
        _userValidator.Validate(Arg.Any<User>()).Returns(errors);

        const string userName = "Nenega";
        Guid userId = Guid.Empty;

        UserDto userDto = new(userName);
        User expectedUser = new(userName, userId);
        
        // Act
        
        Result<User> factoryResult = factory.CreateUserFromUserDto(userDto);
        
        // Assert

        Assert.Null(factoryResult.ValidationErrors);
        Assert.Equivalent(expectedUser, factoryResult.OperationResult);
    }

    [Fact]
    public void CreateUserFromUserDto_InputIsInvalid_ReturnsAnyErrors()
    {
        // Arrange

        ValidationErrors errors = new();
        errors.Errors.Add("has error");
        UserFactory factory = new(_userValidator);
        _userValidator.Validate(Arg.Any<User>()).Returns(errors);

        const string userName = "Nenega";
        Guid userId = Guid.Empty;

        UserDto userDto = new(userName);
        User expectedUser = new(userName, userId);
        
        // Act
        
        Result<User> factoryResult = factory.CreateUserFromUserDto(userDto);
        
        // Assert

        Assert.NotNull(factoryResult.ValidationErrors);
    }
}