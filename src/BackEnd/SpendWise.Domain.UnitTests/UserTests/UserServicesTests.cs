using NSubstitute;
using SpendWise.Domain.Factories.Contracts;
using SpendWise.Domain.Models.GlobalModels;
using SpendWise.Domain.Models.UserModels;
using SpendWise.Domain.Repositories.Contracts;
using SpendWise.Domain.Services;

namespace SpendWise.Domain.UnitTests.UserTests;

public class UserServicesTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly IUserFactory _userFactory = Substitute.For<IUserFactory>();

    [Fact]
    public async Task GetAllUsers_UsersExist_ReturnsAllUsers()
    {
        // Arrange

        List<User> expectedUsers = new()
        {
            new User("Nina", Guid.NewGuid()),
            new User("Angus", Guid.NewGuid()),
            new User("Órion", Guid.NewGuid()),
            new User("Thadeu", Guid.NewGuid()),
            new User("Vicenzo", Guid.NewGuid())
        };
        
        UserManagementService userService = new(_userRepository, _userFactory);
        _userRepository.GetUsers().Returns(Task.FromResult(expectedUsers));
        
        // Act
        
        List<User> testUsers = await userService.GetAllUsers();
        
        // Assert
        
        Assert.Equivalent(expectedUsers, testUsers);
    }

    [Fact]
    public async Task GetAllUsers_NoUsers_ReturnsEmptyList()
    {
        // Arrange

        List<User> expectedUsers = new();
        
        UserManagementService userService = new(_userRepository, _userFactory);
        _userRepository.GetUsers().Returns(Task.FromResult(expectedUsers));
        
        // Act
        
        List<User> testUsers = await userService.GetAllUsers();
        
        // Assert
        
        Assert.Empty(testUsers);
    }
    
    [Fact]
    public async Task GetAllFriends_FriendsExist_ReturnsAllFriends()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        List<User> expectedUsers = new()
        {
            new User("Nina", Guid.NewGuid()),
            new User("Angus", Guid.NewGuid()),
            new User("Órion", Guid.NewGuid()),
            new User("Thadeu", Guid.NewGuid()),
            new User("Vicenzo", Guid.NewGuid())
        };
        
        UserManagementService userService = new(_userRepository, _userFactory);
        _userRepository.GetFriends(userId).Returns(Task.FromResult(expectedUsers));
        
        // Act
        
        List<User> testUsers = await userService.GetFriends(userId);
        
        // Assert
        
        Assert.Equivalent(expectedUsers, testUsers);
    }

    [Fact]
    public async Task GetAllFriends_NoFriends_ReturnsEmptyList()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        List<User> expectedUsers = new();
        
        UserManagementService userService = new(_userRepository, _userFactory);
        _userRepository.GetFriends(userId).Returns(Task.FromResult(expectedUsers));
        
        // Act
        
        List<User> testUsers = await userService.GetFriends(userId);
        
        // Assert
        
        Assert.Empty(testUsers);
    }
}