using Api.Controllers;
using Application.Requests;
using Application.Service.Interfaces;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;
        private readonly Faker _faker;

        public AuthControllerTests()
        {
            _faker = new Faker();
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new RegistrationRequest
            {
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password(8)
            };

            const int expectedUserId = 1; 

            _authServiceMock.Setup(x => x.Register(request))
                .ReturnsAsync(expectedUserId); 

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<CreatedResult>()
                .Which.Location.Should().BeNull(); 

            _authServiceMock.Verify(x => x.Register(request), Times.Once);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ShouldThrowUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "wrong@example.com",
                Password = "wrongPassword"
            };

            _authServiceMock.Setup(x => x.Login(request))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _controller.Login(request));
        }
    }
}