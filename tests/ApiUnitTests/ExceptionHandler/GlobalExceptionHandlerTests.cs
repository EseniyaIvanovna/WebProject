using Api.ExceptionHandler;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;

namespace ApiUnitTests.ExceptionHandler
{
    public class GlobalExceptionHandlerTests
    {
        private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
        private readonly GlobalExceptionHandler _handler;
        private readonly DefaultHttpContext _httpContext;

        public GlobalExceptionHandlerTests()
        {
            _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
            _handler = new GlobalExceptionHandler(_problemDetailsServiceMock.Object);
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task TryHandleAsync_WithAnyException_ShouldHandleException()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
            Assert.Equal("application/problem+json", _httpContext.Response.ContentType);

            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.HttpContext == _httpContext &&
                ctx.ProblemDetails.Status == (int)HttpStatusCode.InternalServerError &&
                ctx.ProblemDetails.Title == "An error occured ..." &&
                ctx.ProblemDetails.Detail == "Test exception" &&
                ctx.ProblemDetails.Instance == _httpContext.Request.Path &&
                ctx.ProblemDetails.Type == nameof(Exception) &&
                ctx.Exception == exception
            )), Times.Once);
        }

        [Fact]
        public async Task TryHandleAsync_WithCustomException_ShouldIncludeExceptionDetails()
        {
            // Arrange
            var exception = new InvalidOperationException("Custom operation failed");

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);

            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Type == nameof(InvalidOperationException) &&
                ctx.ProblemDetails.Detail == "Custom operation failed"
            )), Times.Once);
        }

        [Fact]
        public async Task TryHandleAsync_ShouldMaintainResponseHeaders()
        {
            // Arrange
            var exception = new Exception("Test");
            _httpContext.Response.Headers["Test-Header"] = "Test-Value";

            // Act
            await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            Assert.Equal("Test-Value", _httpContext.Response.Headers["Test-Header"]);
        }
    }
}