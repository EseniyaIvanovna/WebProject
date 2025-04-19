using Application.Exceptions;
using Application.Requests;
using Application.Service;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Services;

[Collection("IntegrationTests")]
public class MessageServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IMessageService _messageService;

    public MessageServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
    }

    [Fact]
    public async Task GetAllMessages_ShouldReturnMessages()
    {
        // Arrange
        var sender = await _fixture.CreateUser();
        var receiver = await _fixture.CreateUser();
        var request1 = new CreateMessageRequest 
        { 
            Text = "Message 1", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };
        await _messageService.Create(request1);

        var request2 = new CreateMessageRequest 
        { 
            Text = "Message 2", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };
        await _messageService.Create(request2);

        // Act
        var messages = (await _messageService.GetAll()).ToList();

        // Assert
        messages.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetMessageById_ShouldReturnMessage()
    {
        // Arrange
        var sender = await _fixture.CreateUser();
        var receiver = await _fixture.CreateUser();
        var request = new CreateMessageRequest 
        { 
            Text = "Test Message", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };
        var messageId = await _messageService.Create(request);

        // Act
        var message = await _messageService.GetById(messageId);

        // Assert
        message.Should().NotBeNull();
        message.Id.Should().Be(messageId);
        message.Text.Should().Be(request.Text);
        message.SenderId.Should().Be(sender.Id);
        message.ReceiverId.Should().Be(receiver.Id);
        message.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task GetMessagesByUserId_ShouldReturnUserMessages()
    {
        // Arrange
        var sender = await _fixture.CreateUser();
        var receiver = await _fixture.CreateUser();
        var request1 = new CreateMessageRequest 
        { 
            Text = "Message 1", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };
        await _messageService.Create(request1);

        var request2 = new CreateMessageRequest 
        { 
            Text = "Message 2", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };
        await _messageService.Create(request2);

        // Act
        var messages = (await _messageService.GetByUserId(sender.Id)).ToList();

        // Assert
        messages.Should().HaveCountGreaterThanOrEqualTo(2);
        messages.Should().OnlyContain(m => m.SenderId == sender.Id || m.ReceiverId == sender.Id);
    }

    [Fact]
    public async Task CreateMessage_ShouldCreateNewMessage()
    {
        // Arrange
        var sender = await _fixture.CreateUser();
        var receiver = await _fixture.CreateUser();
        var request = new CreateMessageRequest 
        { 
            Text = "New Message", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };

        // Act
        var messageId = await _messageService.Create(request);

        // Assert
        messageId.Should().BeGreaterThan(0);
        var createdMessage = await _messageService.GetById(messageId);
        createdMessage.Should().NotBeNull();
        createdMessage.Text.Should().Be(request.Text);
        createdMessage.SenderId.Should().Be(sender.Id);
        createdMessage.ReceiverId.Should().Be(receiver.Id);
        createdMessage.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task UpdateMessage_ShouldUpdateExistingMessage()
    {
        // Arrange
        var sender = await _fixture.CreateUser();
        var receiver = await _fixture.CreateUser();
        var createRequest = new CreateMessageRequest 
        { 
            Text = "Original Message", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };
        var messageId = await _messageService.Create(createRequest);

        var updateRequest = new UpdateMessageRequest 
        { 
            Id = messageId, 
            Text = "Updated Message" 
        };

        // Act
        await _messageService.Update(updateRequest);

        // Assert
        var updatedMessage = await _messageService.GetById(messageId);
        updatedMessage.Should().NotBeNull();
        updatedMessage.Text.Should().Be(updateRequest.Text);
    }

    [Fact]
    public async Task DeleteMessage_ShouldRemoveMessage()
    {
        // Arrange
        var sender = await _fixture.CreateUser();
        var receiver = await _fixture.CreateUser();
        var request = new CreateMessageRequest 
        { 
            Text = "Message to Delete", 
            SenderId = sender.Id,
            ReceiverId = receiver.Id
        };
        var messageId = await _messageService.Create(request);

        // Act
        await _messageService.Delete(messageId);

        // Assert
        await _messageService.Invoking(x => x.GetById(messageId))
            .Should().ThrowAsync<NotFoundApplicationException>()
            .WithMessage($"Message {messageId} not found");
    }
}
