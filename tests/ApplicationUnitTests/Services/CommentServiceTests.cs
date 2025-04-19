using Application.Exceptions;
using Application.Mappings;
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

namespace ApplicationUnitTests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IPostService> _postServiceMock;
        private readonly IMapper _mapper;
        private readonly ICommentService _commentService;
        private readonly Comment _testComment;
        private readonly Mock<ILogger<CommentService>> _loggerMock;
        private readonly Faker _faker;

        public CommentServiceTests()
        {
            _faker = new Faker();

            _testComment = new Comment
            {
                Id = 1,
                PostId = 1,
                UserId = 1,
                Content = _faker.Lorem.Sentence(),
                CreatedAt = DateTime.UtcNow
            };

            _commentRepositoryMock = new Mock<ICommentRepository>();
            _commentRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(_testComment);

            _postServiceMock = new Mock<IPostService>();
            _postServiceMock.Setup(x => x.GetById(_testComment.PostId))
                .ReturnsAsync(new PostResponse 
                { 
                    Id = _testComment.PostId,
                    UserId = _testComment.UserId,
                    Text = _faker.Lorem.Paragraph(),
                    CreatedAt = DateTime.UtcNow
                });

            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mappingConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<CommentService>>();

            _commentService = new CommentService(_commentRepositoryMock.Object, _postServiceMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public void ShouldBeAvailableToCreate()
        {
            // Assert
            _commentService.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ExistingComment_ShouldReturnCommentResponse()
        {
            // Arrange
            _commentRepositoryMock.Setup(x => x.GetById(_testComment.Id))
                .ReturnsAsync(_testComment);

            // Act
            var result = await _commentService.GetById(_testComment.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_testComment.Id);
            result.PostId.Should().Be(_testComment.PostId);
            result.UserId.Should().Be(_testComment.UserId);
            result.Content.Should().Be(_testComment.Content);
            result.CreatedAt.Should().Be(_testComment.CreatedAt);
            _commentRepositoryMock.Verify(x => x.GetById(_testComment.Id), Times.Once);
        }

        [Fact]
        public async Task GetById_NonExistingComment_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _commentRepositoryMock.Setup(x => x.GetById(nonExistingId))
                .ReturnsAsync((Comment?)null);

            // Act & Assert
            await _commentService.Invoking(x => x.GetById(nonExistingId))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Comment {nonExistingId} not found");
        }

        [Fact]
        public async Task Create_ValidRequest_ShouldCreateComment()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                PostId = _testComment.PostId,
                UserId = _testComment.UserId,
                Content = _testComment.Content
            };

            _commentRepositoryMock.Setup(x => x.Create(It.IsAny<Comment>()))
                .ReturnsAsync(_testComment.Id);

            // Act
            var result = await _commentService.Create(request);

            // Assert
            result.Should().Be(_testComment.Id);
            _commentRepositoryMock.Verify(x => x.Create(It.Is<Comment>(c =>
                c.PostId == request.PostId &&
                c.UserId == request.UserId &&
                c.Content == request.Content)), Times.Once);
            _postServiceMock.Verify(x => x.GetById(request.PostId), Times.Once);
        }

        [Fact]
        public async Task Create_NonExistingPost_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                PostId = 999,
                UserId = _testComment.UserId,
                Content = _testComment.Content
            };

            _postServiceMock.Setup(x => x.GetById(request.PostId))
                .ReturnsAsync((PostResponse)null);

            // Act & Assert
            await _commentService.Invoking(x => x.Create(request))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Post {request.PostId} not found");
        }

        [Fact]
        public async Task Update_ExistingComment_ShouldUpdateComment()
        {
            // Arrange
            var request = new UpdateCommentRequest
            {
                Id = _testComment.Id,
                Content = _faker.Lorem.Sentence()
            };

            _commentRepositoryMock.Setup(x => x.GetById(_testComment.Id))
                .ReturnsAsync(_testComment);
            _commentRepositoryMock.Setup(x => x.Update(It.IsAny<Comment>()))
                .ReturnsAsync(true);

            // Act
            await _commentService.Update(request);

            // Assert
            _commentRepositoryMock.Verify(x => x.GetById(_testComment.Id), Times.Once);
            _commentRepositoryMock.Verify(x => x.Update(It.Is<Comment>(c =>
                c.Id == request.Id &&
                c.Content == request.Content)), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingComment_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdateCommentRequest
            {
                Id = 999,
                Content = _faker.Lorem.Sentence()
            };

            _commentRepositoryMock.Setup(x => x.GetById(request.Id))
                .ReturnsAsync((Comment?)null);

            // Act & Assert
            await _commentService.Invoking(x => x.Update(request))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Comment {request.Id} not found");
        }

        [Fact]
        public async Task Delete_ExistingComment_ShouldDeleteComment()
        {
            // Arrange
            _commentRepositoryMock.Setup(x => x.GetById(_testComment.Id))
                .ReturnsAsync(_testComment);
            _commentRepositoryMock.Setup(x => x.Delete(_testComment.Id))
                .ReturnsAsync(true);

            // Act
            await _commentService.Delete(_testComment.Id);

            // Assert
            _commentRepositoryMock.Verify(x => x.GetById(_testComment.Id), Times.Once);
            _commentRepositoryMock.Verify(x => x.Delete(_testComment.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_NonExistingComment_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _commentRepositoryMock.Setup(x => x.GetById(nonExistingId))
                .ReturnsAsync((Comment?)null);

            // Act & Assert
            await _commentService.Invoking(x => x.Delete(nonExistingId))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Comment {nonExistingId} not found");
        }

        [Fact]
        public async Task GetByUserId_ExistingComments_ShouldReturnCommentResponses()
        {
            // Arrange
            var comments = new List<Comment> { _testComment };
            _commentRepositoryMock.Setup(x => x.GetByUserId(_testComment.UserId))
                .ReturnsAsync(comments);

            // Act
            var result = await _commentService.GetByUserId(_testComment.UserId);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            var firstComment = result.First();
            firstComment.Id.Should().Be(_testComment.Id);
            firstComment.PostId.Should().Be(_testComment.PostId);
            firstComment.UserId.Should().Be(_testComment.UserId);
            firstComment.Content.Should().Be(_testComment.Content);
            firstComment.CreatedAt.Should().Be(_testComment.CreatedAt);
            _commentRepositoryMock.Verify(x => x.GetByUserId(_testComment.UserId), Times.Once);
        }

        [Fact]
        public async Task GetAll_NoComments_ShouldReturnEmptyList()
        {
            // Arrange
            _commentRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<Comment>());

            // Act
            var result = await _commentService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _commentRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ExistingComments_ShouldReturnCommentResponses()
        {
            // Arrange
            var comments = new List<Comment> { _testComment };
            _commentRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(comments);

            // Act
            var result = await _commentService.GetAll();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            var firstComment = result.First();
            firstComment.Id.Should().Be(_testComment.Id);
            firstComment.PostId.Should().Be(_testComment.PostId);
            firstComment.UserId.Should().Be(_testComment.UserId);
            firstComment.Content.Should().Be(_testComment.Content);
            firstComment.CreatedAt.Should().Be(_testComment.CreatedAt);
            _commentRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
