using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service;
using AutoMapper;
using Bogus;
using Domain;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ApplicationUnitTests.Services
{
    public class UserServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IInteractionRepository _interactionRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly Faker _faker;

        private readonly IUserService _userService;
        private readonly User _user;

        public UserServiceTests()
        {
            _faker = new Faker();

            _user = new User
            {
                Id = 1,
                Name = _faker.Name.FirstName(),
                LastName = _faker.Name.LastName(),
                Email = _faker.Internet.Email(),
                DateOfBirth = _faker.Date.Past(20),
                Info = _faker.Lorem.Sentence()
            };

            // Mock UserRepository
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(x => x.GetById(_user.Id)).ReturnsAsync(_user);
            userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { _user });
            userRepoMock.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync(1);
            userRepoMock.Setup(x => x.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
            userRepoMock.Setup(x => x.Delete(_user.Id)).Returns(Task.CompletedTask);
            _userRepository = userRepoMock.Object;

            // Mock other repositories
            _postRepository = Mock.Of<IPostRepository>();
            _commentRepository = Mock.Of<ICommentRepository>();
            _reactionRepository = Mock.Of<IReactionRepository>();
            _interactionRepository = Mock.Of<IInteractionRepository>();
            _messageRepository = Mock.Of<IMessageRepository>();

            // AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserResponse>();
                cfg.CreateMap<CreateUserRequest, User>();
                cfg.CreateMap<UpdateUserRequest, User>();
            });
            _mapper = config.CreateMapper();

            _userService = new UserService(
                _userRepository,
                _postRepository,
                _commentRepository,
                _messageRepository,
                _reactionRepository,
                _interactionRepository,
                _mapper,
                Mock.Of<IConfiguration>());
        }

        [Fact]
        public void Service_ShouldBeCreatedSuccessfully()
        {
            _userService.Should().NotBeNull();
        }

        public class GetMethods : UserServiceTests
        {
            [Fact]
            public async Task GetAll_ShouldReturnAllUsers()
            {
                // Act
                var result = await _userService.GetAll();

                // Assert
                result.Should().NotBeNull()
                    .And.HaveCount(1)
                    .And.ContainSingle(u => u.Id == _user.Id);
            }

            [Fact]
            public async Task GetById_ShouldReturnUser_WhenUserExists()
            {
                // Act
                var result = await _userService.GetById(_user.Id);

                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be(_user.Id);
                result.Name.Should().Be(_user.Name);
                result.LastName.Should().Be(_user.LastName);
                result.Email.Should().Be(_user.Email);
            }

            [Fact]
            public async Task GetById_ShouldThrowException_WhenUserDoesNotExist()
            {
                // Arrange
                var nonExistentUserId = -1;
                var userRepoMock = new Mock<IUserRepository>();
                userRepoMock.Setup(x => x.GetById(nonExistentUserId)).ReturnsAsync((User?)null);

                var service = new UserService(
                    userRepoMock.Object,
                    _postRepository,
                    _commentRepository,
                    _messageRepository,
                    _reactionRepository,
                    _interactionRepository,
                    _mapper,
                    Mock.Of<IConfiguration>());

                // Act & Assert
                await service.Invoking(x => x.GetById(nonExistentUserId))
                    .Should().ThrowAsync<NotFoundApplicationException>()
                    .WithMessage($"User {nonExistentUserId} not found");
            }
        }

        public class CreateMethods : UserServiceTests
        {
            [Fact]
            public async Task Add_ShouldReturnNewUserId_WhenRequestIsValid()
            {
                // Arrange
                var request = new CreateUserRequest
                {
                    Name = _faker.Name.FirstName(),
                    LastName = _faker.Name.LastName(),
                    Email = _faker.Internet.Email(),
                    DateOfBirth = _faker.Date.Past(20),
                    Info = _faker.Lorem.Sentence()
                };

                // Act
                var result = await _userService.Add(request);

                // Assert
                result.Should().Be(1);
            }

            [Fact]
            public async Task Add_ShouldMapRequestToUserCorrectly()
            {
                // Arrange
                var request = new CreateUserRequest
                {
                    Name = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Info = "Test info"
                };

                User? createdUser = null;
                var userRepoMock = new Mock<IUserRepository>();
                userRepoMock.Setup(x => x.Create(It.IsAny<User>()))
                    .Callback<User>(u => createdUser = u)
                    .ReturnsAsync(1);

                var service = new UserService(
                    userRepoMock.Object,
                    _postRepository,
                    _commentRepository,
                    _messageRepository,
                    _reactionRepository,
                    _interactionRepository,
                    _mapper,
                    Mock.Of<IConfiguration>());

                // Act
                await service.Add(request);

                // Assert
                createdUser.Should().NotBeNull();
                createdUser.Name.Should().Be(request.Name);
                createdUser.LastName.Should().Be(request.LastName);
                createdUser.Email.Should().Be(request.Email);
                createdUser.DateOfBirth.Should().Be(request.DateOfBirth);
                createdUser.Info.Should().Be(request.Info);
            }
        }

        public class UpdateMethods : UserServiceTests
        {
            [Fact]
            public async Task Update_ShouldCompleteSuccessfully_WhenUserExists()
            {
                // Arrange
                var request = new UpdateUserRequest
                {
                    Id = _user.Id,
                    Name = "Updated Name",
                    LastName = "Updated LastName",
                    Email = "updated@example.com",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Info = "Updated info"
                };

                // Act
                await _userService.Update(request);

                // Assert
                Mock.Get(_userRepository).Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            }

            [Fact]
            public async Task Update_ShouldMapRequestToUserCorrectly()
            {
                // Arrange
                var request = new UpdateUserRequest
                {
                    Id = _user.Id,
                    Name = "Updated",
                    LastName = "User",
                    Email = "updated@example.com",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Info = "Updated info"
                };

                User? updatedUser = null;
                var userRepoMock = new Mock<IUserRepository>();
                userRepoMock.Setup(x => x.GetById(_user.Id)).ReturnsAsync(_user);
                userRepoMock.Setup(x => x.Update(It.IsAny<User>()))
                    .Callback<User>(u => updatedUser = u)
                    .Returns(Task.CompletedTask);

                var service = new UserService(
                    userRepoMock.Object,
                    _postRepository,
                    _commentRepository,
                    _messageRepository,
                    _reactionRepository,
                    _interactionRepository,
                    _mapper,
                    Mock.Of<IConfiguration>());

                // Act
                await service.Update(request);

                // Assert
                updatedUser.Should().NotBeNull();
                updatedUser.Name.Should().Be(request.Name);
                updatedUser.LastName.Should().Be(request.LastName);
                updatedUser.Email.Should().Be(request.Email);
                updatedUser.DateOfBirth.Should().Be(request.DateOfBirth);
                updatedUser.Info.Should().Be(request.Info);
            }

            [Fact]
            public async Task Update_ShouldThrowException_WhenUserDoesNotExist()
            {
                // Arrange
                var nonExistentUserId = -1;
                var request = new UpdateUserRequest { Id = nonExistentUserId, Name="", LastName="" };

                var userRepoMock = new Mock<IUserRepository>();
                userRepoMock.Setup(x => x.GetById(nonExistentUserId)).ReturnsAsync((User?)null);

                var service = new UserService(
                    userRepoMock.Object,
                    _postRepository,
                    _commentRepository,
                    _messageRepository,
                    _reactionRepository,
                    _interactionRepository,
                    _mapper,
                    Mock.Of<IConfiguration>());

                // Act & Assert
                await service.Invoking(x => x.Update(request))
                    .Should().ThrowAsync<NotFoundApplicationException>()
                    .WithMessage($"User {request.Id} not found");
            }
        }

        public class DeleteMethods : UserServiceTests
        {
            [Fact]
            public async Task Delete_ShouldCompleteSuccessfully_WhenUserExists()
            {
                // Act
                await _userService.Delete(_user.Id);

                // Assert
                Mock.Get(_userRepository).Verify(x => x.Delete(_user.Id), Times.Once);
                // Verify all dependencies are called
                Mock.Get(_commentRepository).Verify(x => x.DeleteByUserId(_user.Id), Times.Once);
                Mock.Get(_reactionRepository).Verify(x => x.DeleteByUserId(_user.Id), Times.Once);
                Mock.Get(_postRepository).Verify(x => x.DeleteByUserId(_user.Id), Times.Once);
                Mock.Get(_interactionRepository).Verify(x => x.DeleteByUserId(_user.Id), Times.Once);
                Mock.Get(_messageRepository).Verify(x => x.DeleteMessagesByUser(_user.Id), Times.Once);
            }

            [Fact]
            public async Task Delete_ShouldThrowException_WhenUserDoesNotExist()
            {
                // Arrange
                var nonExistentUserId = -1;
                var userRepoMock = new Mock<IUserRepository>();
                userRepoMock.Setup(x => x.GetById(nonExistentUserId)).ReturnsAsync((User?)null);

                var service = new UserService(
                    userRepoMock.Object,
                    _postRepository,
                    _commentRepository,
                    _messageRepository,
                    _reactionRepository,
                    _interactionRepository,
                    _mapper,
                    Mock.Of<IConfiguration>());

                // Act & Assert
                await service.Invoking(x => x.Delete(nonExistentUserId))
                    .Should().ThrowAsync<NotFoundApplicationException>()
                    .WithMessage($"User {nonExistentUserId} not found");
            }

            [Fact]
            public async Task Delete_ShouldNotCompleteTransaction_WhenDependencyFails()
            {
                // Arrange
                var userRepoMock = new Mock<IUserRepository>();
                userRepoMock.Setup(x => x.GetById(_user.Id)).ReturnsAsync(_user);
                userRepoMock.Setup(x => x.Delete(_user.Id)).Returns(Task.CompletedTask);

                var commentRepoMock = new Mock<ICommentRepository>();
                commentRepoMock.Setup(x => x.DeleteByUserId(_user.Id))
                    .ThrowsAsync(new Exception("Test exception"));

                var service = new UserService(
                    userRepoMock.Object,
                    _postRepository,
                    commentRepoMock.Object,
                    _messageRepository,
                    _reactionRepository,
                    _interactionRepository,
                    _mapper,
                    Mock.Of<IConfiguration>());

                // Act & Assert
                await service.Invoking(x => x.Delete(_user.Id))
                    .Should().ThrowAsync<Exception>()
                    .WithMessage("Test exception");

                // Verify user delete was not called
                userRepoMock.Verify(x => x.Delete(_user.Id), Times.Never);
            }
        }
    }
}

