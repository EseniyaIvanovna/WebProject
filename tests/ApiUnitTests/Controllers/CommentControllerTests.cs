using Api.Controllers;
using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Responses;
using Application.Service;
using AutoMapper;
using Bogus;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers
{
    public class CommentControllerTests
    {
        private readonly Mock<ICommentService> _commentServiceMock;
        private readonly CommentController _controller;
        private readonly Faker _faker;
        private readonly Comment _testComment;
        private readonly IMapper _mapper;

        public CommentControllerTests()
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

            _commentServiceMock = new Mock<ICommentService>();

            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mappingConfig.CreateMapper();

            _controller = new CommentController(_commentServiceMock.Object);
        }

        [Fact]
        public async Task CreateComment_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                PostId = _testComment.PostId,
                UserId = _testComment.UserId,
                Content = _testComment.Content
            };

            _commentServiceMock.Setup(x => x.Create(request))
                .ReturnsAsync(_testComment.Id);

            // Act
            var result = await _controller.CreateComment(request);

            // Assert
            result.Should().BeOfType<CreatedResult>()
                .Which.Location.Should().Be($"/comment/{_testComment.Id}");

            result.Should().BeOfType<CreatedResult>()
                .Which.Value.Should().BeEquivalentTo(new { Id = _testComment.Id });

            _commentServiceMock.Verify(x => x.Create(request), Times.Once);
        }

        [Fact]
        public async Task CreateComment_ServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var request = new CreateCommentRequest() {

                PostId = 0,
                UserId = 0,
                Content = null
            };
            _commentServiceMock.Setup(x => x.Create(request))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await FluentActions
                .Invoking(() => _controller.CreateComment(request))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Test exception");
        }

        [Fact]
        public async Task GetCommentById_ExistingComment_ShouldReturnOkWithComment()
        {
            // Arrange
            var commentResponse = _mapper.Map<CommentResponse>(_testComment);
            _commentServiceMock.Setup(x => x.GetById(_testComment.Id))
                .ReturnsAsync(commentResponse);

            // Act
            var result = await _controller.GetCommentById(_testComment.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(commentResponse);

            _commentServiceMock.Verify(x => x.GetById(_testComment.Id), Times.Once);
        }

        [Fact]
        public async Task GetCommentById_NonExistingComment_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _commentServiceMock.Setup(x => x.GetById(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Comment {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.GetCommentById(nonExistingId));
        }

        [Fact]
        public async Task GetCommentsByUserId_ExistingComments_ShouldReturnOkWithComments()
        {
            // Arrange
            var comments = new List<CommentResponse>
            {
                _mapper.Map<CommentResponse>(_testComment)
            };

            _commentServiceMock.Setup(x => x.GetByUserId(_testComment.UserId))
                .ReturnsAsync(comments);

            // Act
            var result = await _controller.GetCommentsByUserId(_testComment.UserId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(comments);

            _commentServiceMock.Verify(x => x.GetByUserId(_testComment.UserId), Times.Once);
        }

        [Fact]
        public async Task GetCommentsByUserId_NoComments_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _commentServiceMock.Setup(x => x.GetByUserId(It.IsAny<int>()))
                .ReturnsAsync(new List<CommentResponse>());

            // Act
            var result = await _controller.GetCommentsByUserId(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<CommentResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateComment_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var request = new UpdateCommentRequest
            {
                Id = _testComment.Id,
                Content = _faker.Lorem.Sentence()
            };

            // Act
            var result = await _controller.UpdateComment(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _commentServiceMock.Verify(x => x.Update(request), Times.Once);
        }

        [Fact]
        public async Task UpdateComment_NonExistingComment_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdateCommentRequest { Id = 999 };
            _commentServiceMock.Setup(x => x.Update(request))
                .ThrowsAsync(new NotFoundApplicationException($"Comment {request.Id} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.UpdateComment(request));
        }

        [Fact]
        public async Task DeleteComment_ExistingComment_ShouldReturnNoContent()
        {
            // Arrange
            _commentServiceMock.Setup(x => x.Delete(_testComment.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteComment(_testComment.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _commentServiceMock.Verify(x => x.Delete(_testComment.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_NonExistingComment_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _commentServiceMock.Setup(x => x.Delete(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Comment {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.DeleteComment(nonExistingId));
        }

        [Fact]
        public async Task GetAllComments_WithComments_ShouldReturnOkWithComments()
        {
            // Arrange
            var comments = new List<CommentResponse>
            {
                _mapper.Map<CommentResponse>(_testComment)
            };

            _commentServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(comments);

            // Act
            var result = await _controller.GetAllComments();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(comments);

            _commentServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllComments_NoComments_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _commentServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<CommentResponse>());

            // Act
            var result = await _controller.GetAllComments();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<CommentResponse>>().Should().BeEmpty();
        }
    }
}