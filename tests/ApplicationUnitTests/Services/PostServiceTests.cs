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
using Npgsql;

namespace ApplicationUnitTests.Services
{
    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IReactionRepository> _reactionRepositoryMock;
        private readonly Mock<NpgsqlConnection> _connectionMock;
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly Post _testPost;
        private readonly Mock<ILogger<PostService>> _loggerMock;
        private readonly Faker _faker;

        public PostServiceTests()
        {
            _faker = new Faker();

            _testPost = new Post
            {
                Id = 1,
                UserId = 1,
                Text = _faker.Lorem.Paragraph(),
                CreatedAt = DateTime.UtcNow
            };

            _postRepositoryMock = new Mock<IPostRepository>();
            _postRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(_testPost);

            _commentRepositoryMock = new Mock<ICommentRepository>();
            _reactionRepositoryMock = new Mock<IReactionRepository>();
            _connectionMock = new Mock<NpgsqlConnection>();

            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mappingConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<PostService>>();

            _postService = new PostService(
                _postRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _reactionRepositoryMock.Object,
                _connectionMock.Object,
                _mapper);
        }

        [Fact]
        public void ShouldBeAvailableToCreate()
        {
            // Assert
            _postService.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ExistingPost_ShouldReturnPostResponse()
        {
            // Arrange
            _postRepositoryMock.Setup(x => x.GetById(_testPost.Id))
                .ReturnsAsync(_testPost);

            // Act
            var result = await _postService.GetById(_testPost.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_testPost.Id);
            result.UserId.Should().Be(_testPost.UserId);
            result.Text.Should().Be(_testPost.Text);
            result.CreatedAt.Should().Be(_testPost.CreatedAt);
            _postRepositoryMock.Verify(x => x.GetById(_testPost.Id), Times.Once);
        }

        [Fact]
        public async Task GetById_NonExistingPost_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _postRepositoryMock.Setup(x => x.GetById(nonExistingId))
                .ReturnsAsync((Post?)null);

            // Act & Assert
            await _postService.Invoking(x => x.GetById(nonExistingId))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Post {nonExistingId} not found");
        }

        [Fact]
        public async Task Create_ValidRequest_ShouldCreatePost()
        {
            // Arrange
            var request = new CreatePostRequest
            {
                UserId = _testPost.UserId,
                Text = _testPost.Text
            };

            _postRepositoryMock.Setup(x => x.Create(It.IsAny<Post>()))
                .ReturnsAsync(_testPost.Id);

            // Act
            var result = await _postService.Create(request);

            // Assert
            result.Should().Be(_testPost.Id);
            _postRepositoryMock.Verify(x => x.Create(It.Is<Post>(p =>
                p.UserId == request.UserId &&
                p.Text == request.Text)), Times.Once);
        }

        [Fact]
        public async Task Update_ExistingPost_ShouldUpdatePost()
        {
            // Arrange
            var request = new UpdatePostRequest
            {
                Id = _testPost.Id,
                Text = _faker.Lorem.Paragraph()
            };

            _postRepositoryMock.Setup(x => x.GetById(_testPost.Id))
                .ReturnsAsync(_testPost);
            _postRepositoryMock.Setup(x => x.Update(It.IsAny<Post>()))
                .ReturnsAsync(true);

            // Act
            await _postService.Update(request);

            // Assert
            _postRepositoryMock.Verify(x => x.GetById(_testPost.Id), Times.Once);
            _postRepositoryMock.Verify(x => x.Update(It.Is<Post>(p =>
                p.Id == request.Id &&
                p.Text == request.Text)), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingPost_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdatePostRequest
            {
                Id = 999,
                Text = _faker.Lorem.Paragraph()
            };

            _postRepositoryMock.Setup(x => x.GetById(request.Id))
                .ReturnsAsync((Post?)null);

            // Act & Assert
            await _postService.Invoking(x => x.Update(request))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Post {request.Id} not found");
        }

        [Fact]
        public async Task Delete_ExistingPost_ShouldDeletePostAndRelatedData()
        {
            // Arrange
            _postRepositoryMock.Setup(x => x.Delete(_testPost.Id))
                .ReturnsAsync(true);

            // Act
            await _postService.Delete(_testPost.Id);

            // Assert
            _postRepositoryMock.Verify(x => x.Delete(_testPost.Id), Times.Once);
            _commentRepositoryMock.Verify(x => x.DeleteByPostId(_testPost.Id), Times.Once);
            _reactionRepositoryMock.Verify(x => x.DeleteByPostId(_testPost.Id), Times.Once);
        }

        [Fact]
        public async Task GetAll_NoPosts_ShouldReturnEmptyList()
        {
            // Arrange
            _postRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<Post>());

            // Act
            var result = await _postService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _postRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ExistingPosts_ShouldReturnPostResponses()
        {
            // Arrange
            var posts = new List<Post> { _testPost };
            _postRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(posts);

            // Act
            var result = await _postService.GetAll();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            var firstPost = result.First();
            firstPost.Id.Should().Be(_testPost.Id);
            firstPost.UserId.Should().Be(_testPost.UserId);
            firstPost.Text.Should().Be(_testPost.Text);
            firstPost.CreatedAt.Should().Be(_testPost.CreatedAt);
            _postRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
