using Application.Services;
using FluentAssertions;

namespace ApplicationIntegrationTests.Services
{
    [Collection("IntegrationTests")]
    public class PasswordHasherTests : IClassFixture<TestingFixture>
    {
        private readonly IPasswordHasher _passwordHasher;

        public PasswordHasherTests(TestingFixture fixture)
        {
            _passwordHasher = new PasswordHasher();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void VerifyPassword_NullStoredHash_ShouldReturnFalse(string password)
        {
            // Act
            var result = _passwordHasher.VerifyPassword("password", password);

            // Assert
            result.Should().BeFalse();
        }
    }
}