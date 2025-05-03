using Application.Services;
using FluentAssertions;

namespace ApplicationIntegrationTests.Services
{
    [Collection("IntegrationTests")]
    public class BCryptHasherTests : IClassFixture<TestingFixture>
    {
        private readonly IPasswordHasher _passwordHasher;

        public BCryptHasherTests(TestingFixture fixture)
        {
            _passwordHasher = new BCryptHasher();
        }

        [Fact]
        public void HashPassword_ShouldReturnValidHash()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash = _passwordHasher.HashPassword(password);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Should().NotBe(password);
            hash.Length.Should().Be(60); 
        }

        [Fact]
        public void HashPassword_NullPassword_ShouldThrowException()
        {
            // Arrange & Act
            var act = () => _passwordHasher.HashPassword(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var hash = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var wrongPassword = "WrongPassword123!";
            var hash = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_NullStoredHash_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var result = _passwordHasher.VerifyPassword(password, null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_EmptyStoredHash_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var result = _passwordHasher.VerifyPassword(password, "");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_InvalidHashFormat_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var invalidPassword = "invalid_hash";
            var invalidHash = _passwordHasher.HashPassword(invalidPassword);

            // Act
            var result = _passwordHasher.VerifyPassword(password, invalidHash);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("short")]
        [InlineData("longpasswordwithoutcomplexity")]
        [InlineData("12345678")]
        public void HashPassword_WeakPasswords_ShouldStillHash(string weakPassword)
        {
            // Act
            var hash = _passwordHasher.HashPassword(weakPassword);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            _passwordHasher.VerifyPassword(weakPassword, hash).Should().BeTrue();
        }

        [Fact]
        public void HashPassword_ShouldProduceDifferentHashesForSamePassword()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash1 = _passwordHasher.HashPassword(password);
            var hash2 = _passwordHasher.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2); 
            _passwordHasher.VerifyPassword(password, hash1).Should().BeTrue();
            _passwordHasher.VerifyPassword(password, hash2).Should().BeTrue();
        }
    }
}