using Api.Controllers;
using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers
{
    public class PostControllerTests
    {
        private readonly Mock<IPostService> _postServiceMock;
        private readonly PostController _controller;
        private readonly Faker _faker;
        private readonly PostResponse _testPost;

        public PostControllerTests()
        {
            _faker = new Faker();

            _testPost = new PostResponse
            {
                Id = 1,
                UserId = 1,
                Text = _faker.Lorem.Paragraph(),
                CreatedAt = DateTime.UtcNow
            };

            _postServiceMock = new Mock<IPostService>();
            _controller = new PostController(_postServiceMock.Object);
        }

        [Fact]
        public async Task CreatePost_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new CreatePostRequest
            {
                UserId = _testPost.UserId,
                Text = _testPost.Text
            };

            _postServiceMock.Setup(x => x.Create(request))
                .ReturnsAsync(_testPost.Id);

            // Act
            var result = await _controller.CreatePost(request);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedResult>().Subject;

            createdResult.Location.Should().Be($"/post/{_testPost.Id}");
            createdResult.Value.Should().BeEquivalentTo(new { Id = _testPost.Id });

            _postServiceMock.Verify(x => x.Create(request), Times.Once);
        }

        [Fact]
        public async Task GetPostById_ExistingPost_ShouldReturnOkWithPost()
        {
            // Arrange
            _postServiceMock.Setup(x => x.GetById(_testPost.Id))
                .ReturnsAsync(_testPost);

            // Act
            var result = await _controller.GetPostById(_testPost.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(_testPost);

            _postServiceMock.Verify(x => x.GetById(_testPost.Id), Times.Once);
        }

        [Fact]
        public async Task GetPostById_NonExistingPost_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _postServiceMock.Setup(x => x.GetById(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Post {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.GetPostById(nonExistingId));
        }

        [Fact]
        public async Task GetAllPosts_WithPosts_ShouldReturnOkWithPosts()
        {
            // Arrange
            var posts = new List<PostResponse> { _testPost };
            _postServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(posts);

            // Act
            var result = await _controller.GetAllPosts();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(posts);

            _postServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllPosts_NoPosts_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _postServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<PostResponse>());

            // Act
            var result = await _controller.GetAllPosts();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<PostResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task UpdatePost_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var request = new UpdatePostRequest
            {
                Id = _testPost.Id,
                Text = _faker.Lorem.Paragraph()
            };

            // Act
            var result = await _controller.UpdatePost(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _postServiceMock.Verify(x => x.Update(request), Times.Once);
        }

        [Fact]
        public async Task UpdatePost_NonExistingPost_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdatePostRequest { Id = 999 };
            _postServiceMock.Setup(x => x.Update(request))
                .ThrowsAsync(new NotFoundApplicationException($"Post {request.Id} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.UpdatePost(request));
        }

        [Fact]
        public async Task DeletePost_ExistingPost_ShouldReturnNoContent()
        {
            // Arrange
            _postServiceMock.Setup(x => x.Delete(_testPost.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePost(_testPost.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _postServiceMock.Verify(x => x.Delete(_testPost.Id), Times.Once);
        }

        [Fact]
        public async Task DeletePost_NonExistingPost_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _postServiceMock.Setup(x => x.Delete(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Post {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.DeletePost(nonExistingId));
        }
    }
}