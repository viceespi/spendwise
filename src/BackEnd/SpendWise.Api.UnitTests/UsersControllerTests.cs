using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SpendWise.Api.Controllers;
using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Services;
using SpendWise.Domain.Services.Contracts;

namespace SpendWise.Api.UnitTests;

public class UsersControllerTests
{
    private readonly IUserManagementService _userManagementService = Substitute.For<IUserManagementService>();

    [Fact]
    public async Task GetUsers_UsersExist_ReturningOk()
    {
        // Arrange
        UsersController controller = new(_userManagementService);
        
        List<User> expectedUsers = new()
        {
            new User("Nina", Guid.NewGuid()),
            new User("Angus", Guid.NewGuid()),
            new User("Órion", Guid.NewGuid()),
            new User("Thadeu", Guid.NewGuid()),
            new User("Vicenzo", Guid.NewGuid())
        };
        
        _userManagementService.GetAllUsers().Returns(Task.FromResult(expectedUsers));
        
        // Act
        
       IActionResult controllerResult = await controller.GetUsers();
        
        // Assert
        
        Assert.IsType<OkObjectResult>(controllerResult);
    }
    
    [Fact]
    public async Task GetFriends_FriendsExist_ReturningOk()
    {
        // Arrange
        UsersController controller = new(_userManagementService);
        Guid userId = Guid.NewGuid();
        
        List<User> expectedUsers = new()
        {
            new User("Nina", Guid.NewGuid()),
            new User("Angus", Guid.NewGuid()),
            new User("Órion", Guid.NewGuid()),
            new User("Thadeu", Guid.NewGuid()),
            new User("Vicenzo", Guid.NewGuid())
        };
        
        _userManagementService.GetFriends(userId).Returns(Task.FromResult(expectedUsers));
        
        // Act
        
        IActionResult controllerResult = await controller.GetFriends(userId);
        
        // Assert
        
        Assert.IsType<OkObjectResult>(controllerResult);
    }
}