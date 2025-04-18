using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Requests;
using Application.Service;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Services;

public class UserServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IUserService _userService;

    public UserServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers()
    {
        // Arrange
        var request1 = new CreateUserRequest 
        { 
            Name = "John",
            LastName = "Doe",
            DateOfBirth = DateTime.Now.AddYears(-20),
            Info = "Test info 1",
            Email = "john@example.com"
        };
        await _userService.Add(request1);

        var request2 = new CreateUserRequest 
        { 
            Name = "Jane",
            LastName = "Smith",
            DateOfBirth = DateTime.Now.AddYears(-25),
            Info = "Test info 2",
            Email = "jane@example.com"
        };
        await _userService.Add(request2);

        // Act
        var users = (await _userService.GetAll()).ToList();

        // Assert
        users.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser()
    {
        // Arrange
        var request = new CreateUserRequest 
        { 
            Name = "Test",
            LastName = "User",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Info = "Test info",
            Email = "test@example.com"
        };
        var userId = await _userService.Add(request);

        // Act
        var user = await _userService.GetById(userId);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(userId);
        user.Name.Should().Be(request.Name);
        user.LastName.Should().Be(request.LastName);
        user.DateOfBirth.Should().BeCloseTo(request.DateOfBirth, TimeSpan.FromMilliseconds(1));
        user.Info.Should().Be(request.Info);
        user.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task CreateUser_ShouldCreateNewUser()
    {
        // Arrange
        var request = new CreateUserRequest 
        { 
            Name = "New",
            LastName = "User",
            DateOfBirth = DateTime.Now.AddYears(-22),
            Info = "New user info",
            Email = "new@example.com"
        };

        // Act
        var userId = await _userService.Add(request);

        // Assert
        userId.Should().BeGreaterThan(0);
        var createdUser = await _userService.GetById(userId);
        createdUser.Should().NotBeNull();
        createdUser.Name.Should().Be(request.Name);
        createdUser.LastName.Should().Be(request.LastName);
        createdUser.DateOfBirth.Should().BeCloseTo(request.DateOfBirth, TimeSpan.FromMilliseconds(1));
        createdUser.Info.Should().Be(request.Info);
        createdUser.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var createRequest = new CreateUserRequest 
        { 
            Name = "Original",
            LastName = "User",
            DateOfBirth = DateTime.Now.AddYears(-25),
            Info = "Original info",
            Email = "original@example.com"
        };
        var userId = await _userService.Add(createRequest);

        var updateRequest = new UpdateUserRequest 
        { 
            Id = userId,
            Name = "Updated",
            LastName = "User",
            DateOfBirth = DateTime.Now.AddYears(-26),
            Info = "Updated info",
            Email = "updated@example.com"
        };

        // Act
        await _userService.Update(updateRequest);

        // Assert
        var updatedUser = await _userService.GetById(userId);
        updatedUser.Should().NotBeNull();
        updatedUser.Name.Should().Be(updateRequest.Name);
        updatedUser.LastName.Should().Be(updateRequest.LastName);
        updatedUser.DateOfBirth.Should().BeCloseTo(updateRequest.DateOfBirth, TimeSpan.FromMilliseconds(1));
        updatedUser.Info.Should().Be(updateRequest.Info);
        updatedUser.Email.Should().Be(updateRequest.Email);
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteExistingUser()
    {
        // Arrange
        var request = new CreateUserRequest 
        { 
            Name = "Delete",
            LastName = "User",
            DateOfBirth = DateTime.Now.AddYears(-28),
            Info = "To be deleted",
            Email = "delete@example.com"
        };
        var userId = await _userService.Add(request);

        // Act & Assert
        await _userService.Invoking(x => x.Delete(userId))
            .Should().NotThrowAsync();
    }
}
