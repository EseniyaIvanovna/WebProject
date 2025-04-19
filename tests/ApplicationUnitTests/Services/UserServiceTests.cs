using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service;
using AutoMapper;
using Bogus;
using Domain;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;

namespace ApplicationUnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IReactionRepository> _reactionRepositoryMock;
        private readonly Mock<IInteractionRepository> _interactionRepositoryMock;
        private readonly Mock<IMessageRepository> _messageRepositoryMock;
        private readonly Mock<NpgsqlConnection> _connectionMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;
        private readonly Faker _faker;
        private readonly User _testUser;

        public UserServiceTests()
        {
            _faker = new Faker();

            _testUser = new User
            {
                Id = 1,
                Name = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                DateOfBirth = _faker.Person.DateOfBirth,
                Info = _faker.Lorem.Paragraph(),
                Email = _faker.Person.Email
            };

            _connectionMock = new Mock<NpgsqlConnection>("PostgresDB")
            {
                CallBase = true
            };

            _connectionMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<NpgsqlTransaction>());

            _userRepositoryMock = new Mock<IUserRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _reactionRepositoryMock = new Mock<IReactionRepository>();
            _interactionRepositoryMock = new Mock<IInteractionRepository>();
            _messageRepositoryMock = new Mock<IMessageRepository>();

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserRequest, User>();
                cfg.CreateMap<UpdateUserRequest, User>();
                cfg.CreateMap<User, UserResponse>();
            });
            _mapper = mappingConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<UserService>>();
            _userService = new UserService(
                _userRepositoryMock.Object,
                _postRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _messageRepositoryMock.Object,
                _reactionRepositoryMock.Object,
                _interactionRepositoryMock.Object,
                _connectionMock.Object,
                _mapper,
                _loggerMock.Object
                );
        }


        [Fact]
        public void ShouldBeAvailableToCreate()
        {
            // Assert
            _userService.Should().NotBeNull();
        }

        [Fact]
        public async Task Add_ValidRequest_ShouldReturnUserId()
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

            _userRepositoryMock.Setup(x => x.Create(It.IsAny<User>()))
                .ReturnsAsync(_testUser.Id);

            // Act
            var result = await _userService.Add(request);

            // Assert
            result.Should().Be(_testUser.Id);
            _userRepositoryMock.Verify(x => x.Create(It.Is<User>(u =>
                u.Name == request.Name &&
                u.LastName == request.LastName &&
                u.DateOfBirth == request.DateOfBirth &&
                u.Info == request.Info &&
                u.Email == request.Email)), Times.Once);
        }

        [Fact]
        public async Task GetById_ExistingUser_ShouldReturnUserResponse()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetById(_testUser.Id))
                .ReturnsAsync((User)_testUser);

            // Act
            var result = await _userService.GetById(_testUser.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_testUser.Id);
            result.Name.Should().Be(_testUser.Name);
            result.LastName.Should().Be(_testUser.LastName);
            result.DateOfBirth.Should().Be(_testUser.DateOfBirth);
            result.Info.Should().Be(_testUser.Info);
            result.Email.Should().Be(_testUser.Email);
            _userRepositoryMock.Verify(x => x.GetById(_testUser.Id), Times.Once);
        }

        [Fact]
        public async Task GetById_NonExistingUser_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _userRepositoryMock.Setup(x => x.GetById(nonExistingId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await _userService.Invoking(x => x.GetById(nonExistingId))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"User {nonExistingId} not found");
        }

        [Fact]
        public async Task GetAll_WithUsers_ShouldReturnUserResponses()
        {
            // Arrange
            var users = new List<User> { _testUser };
            _userRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAll();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            var firstUser = result.First();
            firstUser.Id.Should().Be(_testUser.Id);
            firstUser.Name.Should().Be(_testUser.Name);
            firstUser.LastName.Should().Be(_testUser.LastName);
            firstUser.DateOfBirth.Should().Be(_testUser.DateOfBirth);
            firstUser.Info.Should().Be(_testUser.Info);
            firstUser.Email.Should().Be(_testUser.Email);
        }

        [Fact]
        public async Task GetAll_NoUsers_ShouldReturnEmptyList()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Update_ValidRequest_ShouldUpdateUser()
        {
            // Arrange
            var request = new UpdateUserRequest
            {
                Id = _testUser.Id,
                Name = "UpdatedName",
                LastName = "UpdatedLastName",
                DateOfBirth = DateTime.Now.AddYears(-30),
                Info = "Updated info",
                Email = "updated@email.com"
            };

            _userRepositoryMock.Setup(x => x.GetById(_testUser.Id))
                .ReturnsAsync(_testUser);
            _userRepositoryMock.Setup(x => x.Update(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            await _userService.Update(request);

            // Assert
            _userRepositoryMock.Verify(x => x.GetById(_testUser.Id), Times.Once);
            _userRepositoryMock.Verify(x => x.Update(It.Is<User>(u =>
                u.Id == request.Id &&
                u.Name == request.Name &&
                u.LastName == request.LastName &&
                u.DateOfBirth == request.DateOfBirth &&
                u.Info == request.Info &&
                u.Email == request.Email)), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSuccess_ShouldCommitTransaction()
        {
            // Arrange
            var transactionMock = new Mock<NpgsqlTransaction>();
            _connectionMock.Setup(x => x.BeginTransactionAsync(default))
                .ReturnsAsync(transactionMock.Object);

            _userRepositoryMock.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            await _userService.Delete(_testUser.Id);

            // Assert
            transactionMock.Verify(x => x.CommitAsync(default), Times.Once);
            transactionMock.Verify(x => x.RollbackAsync(default), Times.Never);
        }
        [Fact]
        public async Task Delete_WhenException_ShouldRollbackTransaction()
        {
            // Arrange
            var transactionMock = new Mock<NpgsqlTransaction>();
            _connectionMock.Setup(x => x.BeginTransactionAsync(default))
                .ReturnsAsync(transactionMock.Object);

            _commentRepositoryMock.Setup(x => x.DeleteByUserId(_testUser.Id))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.Delete(_testUser.Id));

            transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(default), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenUserNotExists_ShouldThrowAndRollback()
        {
            // Arrange
            var transactionMock = new Mock<NpgsqlTransaction>();
            _connectionMock.Setup(x => x.BeginTransactionAsync(default))
                .ReturnsAsync(transactionMock.Object);

            _userRepositoryMock.Setup(x => x.Delete(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<EntityDeleteException>(() => _userService.Delete(_testUser.Id));

            transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }
    }
}