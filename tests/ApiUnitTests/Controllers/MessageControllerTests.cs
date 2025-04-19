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
    public class MessageControllerTests
    {
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly MessageController _controller;
        private readonly Faker _faker;
        private readonly MessageResponse _testMessage;

        public MessageControllerTests()
        {
            _faker = new Faker();

            _testMessage = new MessageResponse
            {
                Id = 1,
                SenderId = 1,
                ReceiverId = 2,
                Text = _faker.Lorem.Sentence(),
                CreatedAt = DateTime.UtcNow
            };

            _messageServiceMock = new Mock<IMessageService>();
            _controller = new MessageController(_messageServiceMock.Object);
        }

        [Fact]
        public async Task CreateMessage_ValidRequest_ShouldReturnCreatedResult()
        {
            // Arrange
            var request = new CreateMessageRequest
            {
                SenderId = _testMessage.SenderId,
                ReceiverId = _testMessage.ReceiverId,
                Text = "_testMessage.Text"
            };

            _messageServiceMock.Setup(x => x.Create(request))
                .ReturnsAsync(_testMessage.Id);

            // Act
            var result = await _controller.CreateMessage(request);

            // Assert
            result.Should().BeOfType<CreatedResult>()
                .Which.Location.Should().Be($"/message/{_testMessage.Id}");

            result.Should().BeOfType<CreatedResult>()
                .Which.Value.Should().BeEquivalentTo(new { Id = _testMessage.Id });

            _messageServiceMock.Verify(x => x.Create(request), Times.Once);
        }

        [Fact]
        public async Task GetMessageById_ExistingMessage_ShouldReturnOkWithMessage()
        {
            // Arrange
            _messageServiceMock.Setup(x => x.GetById(_testMessage.Id))
                .ReturnsAsync(_testMessage);

            // Act
            var result = await _controller.GetMessageById(_testMessage.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(_testMessage);

            _messageServiceMock.Verify(x => x.GetById(_testMessage.Id), Times.Once);
        }

        [Fact]
        public async Task GetMessageById_NonExistingMessage_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _messageServiceMock.Setup(x => x.GetById(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Message {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.GetMessageById(nonExistingId));
        }

        [Fact]
        public async Task GetMessagesByUserId_WithMessages_ShouldReturnOkWithMessages()
        {
            // Arrange
            var messages = new List<MessageResponse> { _testMessage };
            _messageServiceMock.Setup(x => x.GetByUserId(_testMessage.SenderId))
                .ReturnsAsync(messages);

            // Act
            var result = await _controller.GetMessagesByUserId(_testMessage.SenderId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(messages);

            _messageServiceMock.Verify(x => x.GetByUserId(_testMessage.SenderId), Times.Once);
        }

        [Fact]
        public async Task GetMessagesByUserId_NoMessages_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _messageServiceMock.Setup(x => x.GetByUserId(It.IsAny<int>()))
                .ReturnsAsync(new List<MessageResponse>());

            // Act
            var result = await _controller.GetMessagesByUserId(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<MessageResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllMessages_WithMessages_ShouldReturnOkWithMessages()
        {
            // Arrange
            var messages = new List<MessageResponse> { _testMessage };
            _messageServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(messages);

            // Act
            var result = await _controller.GetAllMessages();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(messages);

            _messageServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllMessages_NoMessages_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _messageServiceMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<MessageResponse>());

            // Act
            var result = await _controller.GetAllMessages();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.As<IEnumerable<MessageResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateMessage_ValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var request = new UpdateMessageRequest
            {
                Id = _testMessage.Id,
                Text = _faker.Lorem.Sentence()
            };

            // Act
            var result = await _controller.UpdateMessage(request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _messageServiceMock.Verify(x => x.Update(request), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_NonExistingMessage_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var request = new UpdateMessageRequest { Id = 999 };
            _messageServiceMock.Setup(x => x.Update(request))
                .ThrowsAsync(new NotFoundApplicationException($"Message {request.Id} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.UpdateMessage(request));
        }

        [Fact]
        public async Task DeleteMessage_ExistingMessage_ShouldReturnNoContent()
        {
            // Arrange
            _messageServiceMock.Setup(x => x.Delete(_testMessage.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteMessage(_testMessage.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _messageServiceMock.Verify(x => x.Delete(_testMessage.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_NonExistingMessage_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _messageServiceMock.Setup(x => x.Delete(nonExistingId))
                .ThrowsAsync(new NotFoundApplicationException($"Message {nonExistingId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
                _controller.DeleteMessage(nonExistingId));
        }
    }
}