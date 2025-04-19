using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Service;
using AutoMapper;
using Bogus;
using Domain;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApplicationUnitTests.Services
{
    public class ReactionServiceTests
    {
        private readonly Mock<IReactionRepository> _reactionRepositoryMock;
        private readonly IMapper _mapper;
        private readonly IReactionService _reactionService;
        private readonly Reaction _testReaction;
        private readonly Mock<ILogger<ReactionService>> _loggerMock;
        private readonly Faker _faker;

        public ReactionServiceTests()
        {
            _faker = new Faker();

            var user = new User
            {
                Id = 1,
                Name = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                DateOfBirth = _faker.Person.DateOfBirth,
                Info = _faker.Lorem.Paragraph(),
                Email = _faker.Person.Email
            };

            var post = new Post
            {
                Id = 1,
                UserId = user.Id,
                Text = _faker.Lorem.Paragraph(),
                CreatedAt = DateTime.UtcNow
            };

            _testReaction = new Reaction
            {
                Id = 1,
                UserId = user.Id,
                PostId = post.Id,
                Type = ReactionType.Like
            };

            _reactionRepositoryMock = new Mock<IReactionRepository>();
            _reactionRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(_testReaction);

            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mappingConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<ReactionService>>();
            _reactionService = new ReactionService(_reactionRepositoryMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public void ShouldBeAvailableToCreate()
        {
            // Assert
            _reactionService.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ExistingReaction_ShouldReturnReactionResponse()
        {
            // Arrange
            _reactionRepositoryMock.Setup(x => x.GetById(_testReaction.Id))
                .ReturnsAsync(_testReaction);

            // Act
            var result = await _reactionService.GetById(_testReaction.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_testReaction.Id);
            result.UserId.Should().Be(_testReaction.UserId);
            result.PostId.Should().Be(_testReaction.PostId);
            result.Type.Should().Be(_testReaction.Type.ToString());
            _reactionRepositoryMock.Verify(x => x.GetById(_testReaction.Id), Times.Once);
        }

        [Fact]
        public async Task GetById_NonExistingReaction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _reactionRepositoryMock.Setup(x => x.GetById(nonExistingId))
                .ReturnsAsync((Reaction?)null);

            // Act & Assert
            await _reactionService.Invoking(x => x.GetById(nonExistingId))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Reaction {nonExistingId} not found");
        }

        [Fact]
        public async Task Create_ValidRequest_ShouldCreateReaction()
        {
            // Arrange
            var request = new CreateReactionRequest
            {
                UserId = _testReaction.UserId,
                PostId = _testReaction.PostId,
                Type = _testReaction.Type
            };

            _reactionRepositoryMock.Setup(x => x.Exists(request.UserId, request.PostId))
                .ReturnsAsync(false);
            _reactionRepositoryMock.Setup(x => x.Create(It.IsAny<Reaction>()))
                .ReturnsAsync(_testReaction.Id);

            // Act
            var result = await _reactionService.Create(request);

            // Assert
            result.Should().Be(_testReaction.Id);
            _reactionRepositoryMock.Verify(x => x.Create(It.Is<Reaction>(r =>
                r.UserId == request.UserId &&
                r.PostId == request.PostId &&
                r.Type == request.Type)), Times.Once);
        }

        [Fact]
        public async Task Create_ExistingReaction_ShouldThrowConflictApplicationException()
        {
            // Arrange
            var request = new CreateReactionRequest
            {
                UserId = _testReaction.UserId,
                PostId = _testReaction.PostId,
                Type = _testReaction.Type
            };

            _reactionRepositoryMock.Setup(x => x.Exists(request.UserId, request.PostId))
                .ReturnsAsync(true);

            // Act & Assert
            await _reactionService.Invoking(x => x.Create(request))
                .Should().ThrowAsync<ConflictApplicationException>()
                .WithMessage("User can have only one reaction per post");
        }

        [Fact]
        public async Task Update_ExistingReaction_ShouldUpdateReaction()
        {
            // Arrange
            var request = new UpdateReactionRequest
            {
                Id = _testReaction.Id,
                Type = ReactionType.Dislike
            };

            _reactionRepositoryMock.Setup(x => x.GetById(request.Id))
                .ReturnsAsync(_testReaction);
            _reactionRepositoryMock.Setup(x => x.Update(It.IsAny<Reaction>()))
                .ReturnsAsync(true);

            // Act
            await _reactionService.Update(request);

            // Assert
            _reactionRepositoryMock.Verify(x => x.GetById(request.Id), Times.Once);
            _reactionRepositoryMock.Verify(x => x.Update(It.Is<Reaction>(r =>
                r.Id == request.Id &&
                r.Type == request.Type)), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingReaction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdateReactionRequest
            {
                Id = 999,
                Type = ReactionType.Dislike
            };

            _reactionRepositoryMock.Setup(x => x.GetById(request.Id))
                .ReturnsAsync((Reaction?)null);

            // Act & Assert
            await _reactionService.Invoking(x => x.Update(request))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Reaction {request.Id} not found");
        }

        [Fact]
        public async Task Delete_ExistingReaction_ShouldDeleteReaction()
        {
            // Arrange
            _reactionRepositoryMock.Setup(x => x.GetById(_testReaction.Id))
                .ReturnsAsync(_testReaction);
            _reactionRepositoryMock.Setup(x => x.Delete(_testReaction.Id))
                .ReturnsAsync(true);

            // Act
            await _reactionService.Delete(_testReaction.Id);

            // Assert
            _reactionRepositoryMock.Verify(x => x.GetById(_testReaction.Id), Times.Once);
            _reactionRepositoryMock.Verify(x => x.Delete(_testReaction.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_NonExistingReaction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _reactionRepositoryMock.Setup(x => x.GetById(nonExistingId))
                .ReturnsAsync((Reaction?)null);

            // Act & Assert
            await _reactionService.Invoking(x => x.Delete(nonExistingId))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Reaction {nonExistingId} not found");
        }

        [Fact]
        public async Task GetByPostId_ExistingPost_ShouldReturnReactions()
        {
            // Arrange
            var reactions = new List<Reaction> { _testReaction };
            _reactionRepositoryMock.Setup(x => x.GetByPostId(_testReaction.PostId))
                .ReturnsAsync(reactions);

            // Act
            var result = await _reactionService.GetByPostId(_testReaction.PostId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var firstReaction = result.First();
            firstReaction.Id.Should().Be(_testReaction.Id);
            firstReaction.UserId.Should().Be(_testReaction.UserId);
            firstReaction.PostId.Should().Be(_testReaction.PostId);
            firstReaction.Type.Should().Be(_testReaction.Type.ToString());
            _reactionRepositoryMock.Verify(x => x.GetByPostId(_testReaction.PostId), Times.Once);
        }

        [Fact]
        public async Task GetByUserId_ExistingUser_ShouldReturnReactions()
        {
            // Arrange
            var reactions = new List<Reaction> { _testReaction };
            _reactionRepositoryMock.Setup(x => x.GetByUserId(_testReaction.UserId))
                .ReturnsAsync(reactions);

            // Act
            var result = await _reactionService.GetByUserId(_testReaction.UserId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var firstReaction = result.First();
            firstReaction.Id.Should().Be(_testReaction.Id);
            firstReaction.UserId.Should().Be(_testReaction.UserId);
            firstReaction.PostId.Should().Be(_testReaction.PostId);
            firstReaction.Type.Should().Be(_testReaction.Type.ToString());
            _reactionRepositoryMock.Verify(x => x.GetByUserId(_testReaction.UserId), Times.Once);
        }

        [Fact]
        public async Task GetAll_NoReactions_ShouldReturnEmptyList()
        {
            // Arrange
            _reactionRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<Reaction>());

            // Act
            var result = await _reactionService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _reactionRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ExistingReactions_ShouldReturnReactions()
        {
            // Arrange
            var reactions = new List<Reaction> { _testReaction };
            _reactionRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(reactions);

            // Act
            var result = await _reactionService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var firstReaction = result.First();
            firstReaction.Id.Should().Be(_testReaction.Id);
            firstReaction.UserId.Should().Be(_testReaction.UserId);
            firstReaction.PostId.Should().Be(_testReaction.PostId);
            firstReaction.Type.Should().Be(_testReaction.Type.ToString());
            _reactionRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
