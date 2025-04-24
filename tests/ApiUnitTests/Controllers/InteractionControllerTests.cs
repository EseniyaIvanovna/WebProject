using Api.Controllers;
using Application.Exceptions;
using Application.Requests;
using Application.Responses;
using Application.Service;
using Bogus;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers
{
    public class InteractionControllerTests
    {
        private readonly Mock<IInteractionService> _interactionServiceMock;
        private readonly InteractionController _controller;
        private readonly Faker _faker;
        private readonly InteractionResponse _testInteraction;

        public InteractionControllerTests()
        {
            _faker = new Faker();

            _testInteraction = new InteractionResponse
            {
                Id = 1,
                User1Id = 1,
                User2Id = 2,
                Status = Status.Friend,
            };

            _interactionServiceMock = new Mock<IInteractionService>();
            _controller = new InteractionController(_interactionServiceMock.Object);
        }

        [Fact]
        public async Task CreateInteraction_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new CreateInteractionRequest
            {
                User1Id = _testInteraction.User1Id,
                User2Id = _testInteraction.User2Id,
                Status = _testInteraction.Status
            };

            _interactionServiceMock.Setup(x => x.Create(request))
                .ReturnsAsync(_testInteraction.Id);

            // Act
            var result = await _controller.CreateInteraction(request);

            // Assert
            result.Should().BeOfType<CreatedResult>()
                .Which.Location.Should().Be($"/interaction/{_testInteraction.Id}");

            result.Should().BeOfType<CreatedResult>()
                .Which.Value.Should().BeEquivalentTo(new { Id = _testInteraction.Id });

            _interactionServiceMock.Verify(x => x.Create(request), Times.Once);
        }

        [Fact]
        public async Task CreateInteraction_ServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var request = new CreateInteractionRequest();
            _interactionServiceMock.Setup(x => x.Create(request))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await FluentActions
                .Invoking(() => _controller.CreateInteraction(request))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Test exception");
        }

        [Fact]
        public async Task GetInteractionById_ExistingInteraction_ShouldReturnOkWithInteraction()
        {
            // Arrange
            _interactionServiceMock.Setup(x => x.GetById(_testInteraction.Id))
                .ReturnsAsync(_testInteraction);

            // Act
            var result = await _controller.GetInteractionById(_testInteraction.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(_testInteraction);

            _interactionServiceMock.Verify(x => x.GetById(_testInteraction.Id), Times.Once);
        }

        [Fact]
        public async Task GetInteractionById_NonExistingInteraction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _interactionServiceMock.Setup(x => x.GetById(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Interaction {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.GetInteractionById(nonExistingId));
        }

        [Fact]
        public async Task GetInteractionsByStatus_WithInteractions_ShouldReturnOkWithInteractions()
        {
            // Arrange
            var interactions = new List<InteractionResponse> { _testInteraction };
            _interactionServiceMock.Setup(x => x.GetByStatus(_testInteraction.Status))
                .ReturnsAsync(interactions);

            // Act
            var result = await _controller.GetInteractionsByStatus(_testInteraction.Status);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(interactions);

            _interactionServiceMock.Verify(x => x.GetByStatus(_testInteraction.Status), Times.Once);
        }

        [Fact]
        public async Task GetInteractionsByStatus_NoInteractions_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _interactionServiceMock.Setup(x => x.GetByStatus(It.IsAny<Status>()))
                .ReturnsAsync(new List<InteractionResponse>());

            // Act
            var result = await _controller.GetInteractionsByStatus(Status.Friend);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<InteractionResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllInteractions_WithInteractions_ShouldReturnOkWithInteractions()
        {
            // Arrange
            var interactions = new List<InteractionResponse> { _testInteraction };
            _interactionServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(interactions);

            // Act
            var result = await _controller.GetAllInteractions();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(interactions);

            _interactionServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllInteractions_NoInteractions_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _interactionServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<InteractionResponse>());

            // Act
            var result = await _controller.GetAllInteractions();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<InteractionResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateInteraction_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var request = new UpdateInteractionRequest
            {
                Id = _testInteraction.Id,
                Status = Status.Blocked
            };

            // Act
            var result = await _controller.UpdateInteraction(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _interactionServiceMock.Verify(x => x.Update(request), Times.Once);
        }

        [Fact]
        public async Task UpdateInteraction_NonExistingInteraction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdateInteractionRequest { Id = 999 };
            _interactionServiceMock.Setup(x => x.Update(request))
                .ThrowsAsync(new NotFoundApplicationException($"Interaction {request.Id} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.UpdateInteraction(request));
        }

        [Fact]
        public async Task DeleteInteraction_ExistingInteraction_ShouldReturnNoContent()
        {
            // Arrange
            _interactionServiceMock.Setup(x => x.Delete(_testInteraction.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteInteraction(_testInteraction.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _interactionServiceMock.Verify(x => x.Delete(_testInteraction.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteInteraction_NonExistingInteraction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _interactionServiceMock.Setup(x => x.Delete(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Interaction {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.DeleteInteraction(nonExistingId));
        }
    }
}