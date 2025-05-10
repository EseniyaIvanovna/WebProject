using Api.Controllers;
using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service.Interfaces;
using Bogus;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers
{
    public class ReactionControllerTests
    {
        private readonly Mock<IReactionService> _reactionServiceMock;
        private readonly ReactionController _controller;
        private readonly Faker _faker;
        private readonly ReactionResponse _testReaction;

        public ReactionControllerTests()
        {
            _faker = new Faker();

            _testReaction = new ReactionResponse
            {
                Id = 1,
                UserId = 1,
                PostId = 1,
                Type = "Like"
            };

            _reactionServiceMock = new Mock<IReactionService>();
            _controller = new ReactionController(_reactionServiceMock.Object);
        }

        [Fact]
        public async Task CreateReaction_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new CreateReactionRequest
            {
                UserId = _testReaction.UserId,
                PostId = _testReaction.PostId,
                Type = ReactionType.Like
            };

            _reactionServiceMock.Setup(x => x.Create(request))
                .ReturnsAsync(_testReaction.Id);

            // Act
            var result = await _controller.CreateReaction(request);

            // Assert
            result.Should().BeOfType<CreatedResult>()
                .Which.Location.Should().Be($"/reaction/{_testReaction.Id}");

            result.Should().BeOfType<CreatedResult>()
                .Which.Value.Should().BeEquivalentTo(new { Id = _testReaction.Id });

            _reactionServiceMock.Verify(x => x.Create(request), Times.Once);
        }

        [Fact]
        public async Task CreateReaction_ServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var request = new CreateReactionRequest();
            _reactionServiceMock.Setup(x => x.Create(request))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateReaction(request));
        }

        [Fact]
        public async Task GetReactionById_ExistingReaction_ShouldReturnOkWithReaction()
        {
            // Arrange
            _reactionServiceMock.Setup(x => x.GetById(_testReaction.Id))
                .ReturnsAsync(_testReaction);

            // Act
            var result = await _controller.GetReactionById(_testReaction.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(_testReaction);

            _reactionServiceMock.Verify(x => x.GetById(_testReaction.Id), Times.Once);
        }

        [Fact]
        public async Task GetReactionById_NonExistingReaction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _reactionServiceMock.Setup(x => x.GetById(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Reaction {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.GetReactionById(nonExistingId));
        }

        [Fact]
        public async Task GetReactionsByUserId_WithReactions_ShouldReturnOkWithReactions()
        {
            // Arrange
            var reactions = new List<ReactionResponse> { _testReaction };
            _reactionServiceMock.Setup(x => x.GetByUserId(_testReaction.UserId))
                .ReturnsAsync(reactions);

            // Act
            var result = await _controller.GetReactionsByUserId(_testReaction.UserId);

            // Assert
            _reactionServiceMock.Verify(x => x.GetByUserId(_testReaction.UserId), Times.Once);
        }

        [Fact]
        public async Task UpdateReaction_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var request = new UpdateReactionRequest
            {
                Id = _testReaction.Id,
                Type = ReactionType.Dislike
            };

            // Act
            var result = await _controller.UpdateReaction(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _reactionServiceMock.Verify(x => x.Update(request), Times.Once);
        }

        [Fact]
        public async Task UpdateReaction_NonExistingReaction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdateReactionRequest { Id = 999 };
            _reactionServiceMock.Setup(x => x.Update(request))
                .ThrowsAsync(new NotFoundApplicationException($"Reaction {request.Id} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.UpdateReaction(request));
        }

        [Fact]
        public async Task DeleteReaction_ExistingReaction_ShouldReturnNoContent()
        {
            // Arrange
            _reactionServiceMock.Setup(x => x.Delete(_testReaction.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteReaction(_testReaction.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _reactionServiceMock.Verify(x => x.Delete(_testReaction.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteReaction_NonExistingReaction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _reactionServiceMock.Setup(x => x.Delete(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Reaction {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.DeleteReaction(nonExistingId));
        }

        [Fact]
        public async Task GetAllReactions_WithReactions_ShouldReturnOkWithReactions()
        {
            // Arrange
            var reactions = new List<ReactionResponse> { _testReaction };
            _reactionServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(reactions);

            // Act
            var result = await _controller.GetAllReactions();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(reactions);

            _reactionServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllReactions_NoReactions_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _reactionServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<ReactionResponse>());

            // Act
            var result = await _controller.GetAllReactions();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<ReactionResponse>>().Should().BeEmpty();
        }
    }
}