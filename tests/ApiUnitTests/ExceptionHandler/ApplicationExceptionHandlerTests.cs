using Api.ExceptionHandler;
using Application.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ApiUnitTests.ExceptionHandler
{
    public class ApplicationExceptionHandlerTests
    {
        private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
        private readonly ApplicationExceptionHandler _handler;
        private readonly DefaultHttpContext _httpContext;

        public ApplicationExceptionHandlerTests()
        {
            _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
            _handler = new ApplicationExceptionHandler(_problemDetailsServiceMock.Object);
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task TryHandleAsync_WithNonApplicationException_ShouldNotHandleException()
        {
            // Arrange
            var exception = new Exception("Regular exception");

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            result.Should().BeFalse("operation was expected to fail or return false");
            _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK, "200 OK is the expected default status");
            _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
        }

        [Fact]
        public async Task TryHandleAsync_WithDifferentStatusCode_ShouldSetCorrectStatusCode()
        {
            // Arrange
            var exception = new NotFoundApplicationException("Not found");

            // Act
            var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(StatusCodes.Status404NotFound, _httpContext.Response.StatusCode);
        }
    }
}