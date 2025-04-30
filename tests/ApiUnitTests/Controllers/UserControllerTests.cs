using Api.Controllers;
using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;
        private readonly Faker _faker;
        private readonly UserResponse _testUser;

        public UserControllerTests()
        {
            _faker = new Faker();

            _testUser = new UserResponse
            {
                Id = 1,
                Name = _faker.Name.FirstName(),
                LastName = _faker.Name.LastName(),
                DateOfBirth = _faker.Date.Past(20),
                Info = _faker.Lorem.Sentence(),
                Email = _faker.Internet.Email()
            };

            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUserById_ExistingUser_ShouldReturnOkWithUser()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetById(_testUser.Id))
                .ReturnsAsync(_testUser);

            // Act
            var result = await _controller.GetUserById(_testUser.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(_testUser);

            _userServiceMock.Verify(x => x.GetById(_testUser.Id), Times.Once);
        }

        [Fact]
        public async Task GetUserById_NonExistingUser_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _userServiceMock.Setup(x => x.GetById(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"User {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.GetUserById(nonExistingId));
        }

        [Fact]
        public async Task GetAllUsers_WithUsers_ShouldReturnOkWithUsers()
        {
            // Arrange
            var users = new List<UserResponse> { _testUser };
            _userServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(users);

            _userServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllUsers_NoUsers_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<UserResponse>());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<UserResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task CreateUser_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Name = _testUser.Name,
                LastName = _testUser.LastName,
                DateOfBirth = _testUser.DateOfBirth,
                Info = _testUser.Info,
                Email = _testUser.Email
            };

            _userServiceMock.Setup(x => x.Add(request))
                .ReturnsAsync(_testUser.Id);

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            result.Should().BeOfType<CreatedResult>()
                .Which.Location.Should().Be($"/user/{_testUser.Id}");

            result.Should().BeOfType<CreatedResult>()
                .Which.Value.Should().BeEquivalentTo(new { Id = _testUser.Id });

            _userServiceMock.Verify(x => x.Add(request), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var request = new CreateUserRequest()
            {
                Name = "Bad",
                LastName = "Request"
            }; 
            _userServiceMock.Setup(x => x.Add(request))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateUser(request));
        }

        [Fact]
        public async Task UpdateUser_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var request = new UpdateUserRequest
            {
                Id = _testUser.Id,
                Name = _testUser.Name,
                LastName = _testUser.LastName,
                DateOfBirth = _testUser.DateOfBirth,
                Info = _testUser.Info,
                Email = _testUser.Email
            };

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _userServiceMock.Verify(x => x.Update(request), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_NonExistingUser_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdateUserRequest
            {
                Name = "Bad",
                LastName = "Request"
            }; 
            _userServiceMock.Setup(x => x.Update(request))
                .ThrowsAsync(new NotFoundApplicationException($"User {request.Id} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.UpdateUser(request));
        }

        [Fact]
        public async Task DeleteUser_ExistingUser_ShouldReturnNoContent()
        {
            // Arrange
            _userServiceMock.Setup(x => x.Delete(_testUser.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(_testUser.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _userServiceMock.Verify(x => x.Delete(_testUser.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_NonExistingUser_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _userServiceMock.Setup(x => x.Delete(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"User {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.DeleteUser(nonExistingId));
        }
    }
}
