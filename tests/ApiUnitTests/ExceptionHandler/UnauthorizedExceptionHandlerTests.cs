using Api.ExceptionHandler;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ApiUnitTests.ExceptionHandler
{
    public class UnauthorizedExceptionHandlerTests
    {
        private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
        private readonly UnauthorizedExceptionHandler _handler;
        private readonly DefaultHttpContext _httpContext;

        public UnauthorizedExceptionHandlerTests()
        {
            _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
            _handler = new UnauthorizedExceptionHandler(_problemDetailsServiceMock.Object);
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task TryHandleAsync_WithUnauthorizedAccessException_ShouldHandleException()
        {
            // Arrange
            var exception = new UnauthorizedAccessException("Access denied");

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.HttpContext == _httpContext &&
                ctx.ProblemDetails.Status == StatusCodes.Status401Unauthorized &&
                ctx.ProblemDetails.Title == "Access denied" &&
                ctx.ProblemDetails.Detail == "You don't have permission to access this resource" &&
                ctx.ProblemDetails.Instance == _httpContext.Request.Path &&
                ctx.ProblemDetails.Type == nameof(UnauthorizedAccessException) &&
                ctx.Exception == exception
            )));
        }

        [Fact]
        public async Task TryHandleAsync_WithOtherException_ShouldNotHandleException()
        {
            // Arrange
            var exception = new Exception("Generic error");

            // Act
            var result = await _handler.TryHandleAsync( _httpContext, exception, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
        }

        [Fact]
        public async Task TryHandleAsync_ShouldSetCorrectProblemDetailsType()
        {
            // Arrange
            var exception = new UnauthorizedAccessException();

            // Act
            await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Type == nameof(UnauthorizedAccessException)
            )), Times.Once);
        }
    }
}