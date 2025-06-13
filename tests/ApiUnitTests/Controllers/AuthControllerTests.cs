using Api.Controllers;
using Application.Requests;
using Application.Service.Interfaces;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

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

            // Создаем реальный ClaimsPrincipal
            var identity = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, "1"), // ID пользователя
        new Claim(ClaimTypes.Email, request.Email)
    }, "TestAuth");

            var expectedPrincipal = new ClaimsPrincipal(identity);

            _authServiceMock.Setup(x => x.Register(request))
                .ReturnsAsync(expectedPrincipal); // Возвращаем ClaimsPrincipal

            var authService = new Mock<IAuthenticationService>();
            authService.Setup(x => x.SignInAsync(It.IsAny<HttpContext>(),
                                              It.IsAny<string>(),
                                              It.IsAny<ClaimsPrincipal>(),
                                              It.IsAny<AuthenticationProperties>()))
                      .Returns(Task.CompletedTask);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IAuthenticationService)))
                         .Returns(authService.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider.Object
                }
            };

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