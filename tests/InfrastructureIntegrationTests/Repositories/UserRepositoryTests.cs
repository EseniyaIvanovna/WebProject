using Bogus;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureIntegrationTests.Repositories;

[Collection("IntegrationTests")]
public class UserRepositoryTests : IClassFixture<TestingFixture>
{
    private readonly IUserRepository _userRepository;
    private readonly TestingFixture _fixture;
    private readonly Faker _faker;

    public UserRepositoryTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _faker = new Faker();
    }

    [Fact]
    public async Task GetById_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = await _fixture.CreateUser();

        // Act
        var result = await _userRepository.GetById(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetById_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentUserId = 999999;

        // Act
        var result = await _userRepository.GetById(nonExistentUserId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_WhenUsersExist_ReturnsUsers()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();

        var user2 = await _fixture.CreateUser();

        // Act
        var users = (await _userRepository.GetAll()).ToList();

        // Assert
        users.Should().HaveCountGreaterThanOrEqualTo(2);
        users.Should().Contain(u => u.Name == user1.Name && u.Email == user1.Email);
        users.Should().Contain(u => u.Name == user2.Name && u.Email == user2.Email);
    }

    [Fact]
    public async Task Create_WhenValidUser_CreatesUser()
    {
        // Arrange
        var user = await _fixture.CreateUser();

        // Assert
        user.Id.Should().BeGreaterThan(0);
        var createdUser = await _userRepository.GetById(user.Id);
        createdUser.Should().NotBeNull();
        createdUser!.Name.Should().Be(user.Name);
        createdUser.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Update_WhenUserExists_UpdatesUser()
    {
        // Arrange
        var user = await _fixture.CreateUser();

        var updatedUser = user;
        updatedUser.Info="updated info";

        // Act
        var result = await _userRepository.Update(updatedUser);

        // Assert
        result.Should().BeTrue();
        var retrievedUser = await _userRepository.GetById(user.Id);
        retrievedUser.Should().NotBeNull();
        retrievedUser!.Name.Should().Be(updatedUser.Name);
        retrievedUser.Email.Should().Be(updatedUser.Email);
    }

    [Fact]
    public async Task Update_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentUser = await _fixture.CreateUser();
        nonExistentUser.Id = 999999;

        // Act
        var result = await _userRepository.Update(nonExistentUser);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_WhenUserExists_DeletesUser()
    {
        // Arrange
        var user = await _fixture.CreateUser();

        // Act
        var result = await _userRepository.Delete(user.Id);

        // Assert
        result.Should().BeTrue();
        var deletedUser = await _userRepository.GetById(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task Delete_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentUserId = 999999;

        // Act
        var result = await _userRepository.Delete(nonExistentUserId);

        // Assert
        result.Should().BeFalse();
    }
}