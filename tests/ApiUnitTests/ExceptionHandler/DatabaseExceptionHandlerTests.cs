using Api.ExceptionHandler;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Data.Common;

namespace ApiUnitTests.ExceptionHandler
{
    public class DatabaseExceptionHandlerTests
    {
        private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
        private readonly DatabaseExceptionHandler _handler;
        private readonly DefaultHttpContext _httpContext;

        public DatabaseExceptionHandlerTests()
        {
            _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
            _handler = new DatabaseExceptionHandler(_problemDetailsServiceMock.Object);
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task TryHandleAsync_WithDbException_ShouldHandleException()
        {
            // Arrange
            var exception = new Mock<DbException>().Object;

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);            
            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.HttpContext == _httpContext &&
                ctx.ProblemDetails.Status == StatusCodes.Status400BadRequest &&
                ctx.ProblemDetails.Title == "Database error" &&
                ctx.ProblemDetails.Detail == "An error occurred while processing your request in the database" &&
                ctx.ProblemDetails.Instance == _httpContext.Request.Path &&
                ctx.ProblemDetails.Type == exception.GetType().Name &&
                ctx.Exception == exception
            )));
        }

        [Fact]
        public async Task TryHandleAsync_WithNonDbException_ShouldNotHandleException()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);            
            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
        }

        [Fact]
        public async Task TryHandleAsync_WithDbExceptionSubclass_ShouldHandleException()
        {
            // Arrange
            var exception = new CustomDbException();

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Once);
        }

        private class CustomDbException : DbException { }
    }
}