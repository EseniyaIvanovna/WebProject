using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
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
    public class MessageServiceTests
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;
        private readonly Message _message;
        private readonly Mock<ILogger<MessageService>> _loggerMock;

        public MessageServiceTests()
        {
            var faker = new Faker();

            var sender = new User
            {
                Id = 1,
                Name = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                DateOfBirth = faker.Person.DateOfBirth,
                Info = faker.Lorem.Paragraph(),
                Email = faker.Person.Email
            };

            var receiver = new User
            {
                Id = 2,
                Name = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                DateOfBirth = faker.Person.DateOfBirth,
                Info = faker.Lorem.Paragraph(),
                Email = faker.Person.Email
            };

            _message = new Message
            {
                Id = 1,
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Text = faker.Lorem.Sentence(),
                CreatedAt = DateTime.UtcNow
            };

            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(_message);
            _messageRepository = messageRepositoryMock.Object;

            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mappingConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<MessageService>>();
            _messageService = new MessageService(_messageRepository, _mapper);
        }

        [Fact]
        public void ShouldBeAvailableToCreate()
        {
            // Assert
            _messageService.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ShouldReturnMessage()
        {
            // Arrange
            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(x => x.GetById(1)).ReturnsAsync(_message);
            var messageRepository = messageRepositoryMock.Object;
            var messageService = new MessageService(messageRepository, _mapper);
            int messageId = _message.Id;

            // Act
            var messageDto = await messageService.GetById(messageId);

            // Assert
            messageDto.Should().NotBeNull();
            messageDto.Id.Should().Be(_message.Id);
            messageDto.Text.Should().Be(_message.Text);
            messageDto.SenderId.Should().Be(_message.SenderId);
            messageDto.ReceiverId.Should().Be(_message.ReceiverId);
            messageRepositoryMock.Verify(x => x.GetById(1), Times.Once);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2, 2, 3)]
        [InlineData(3, 3, 4)]
        public async Task GetById_WhenRandomId_ShouldReturnMessage(int messageId, int senderId, int receiverId)
        {
            // Arrange
            var faker = new Faker();
            var message = new Message
            {
                Id = messageId,
                SenderId = senderId,
                ReceiverId = receiverId,
                Text = faker.Lorem.Sentence(),
                CreatedAt = DateTime.UtcNow
            };

            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(x => x.GetById(message.Id)).ReturnsAsync(message);
            var messageRepository = messageRepositoryMock.Object;
            var messageService = new MessageService(messageRepository, _mapper);

            // Act
            var messageDto = await messageService.GetById(messageId);

            // Assert
            messageDto.Should().NotBeNull();
            messageDto.Id.Should().Be(message.Id);
            messageDto.Text.Should().Be(message.Text);
            messageDto.SenderId.Should().Be(message.SenderId);
            messageDto.ReceiverId.Should().Be(message.ReceiverId);
            messageRepositoryMock.Verify(x => x.GetById(message.Id), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoMessages_ReturnsEmptyList()
        {
            // Arrange
            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Message>());
            var messageRepository = messageRepositoryMock.Object;
            var messageService = new MessageService(messageRepository, _mapper);

            // Act
            var result = await messageService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            messageRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task Create_ValidRequest_ShouldCreateMessage()
        {
            // Arrange
            var faker = new Faker();
            var request = new CreateMessageRequest
            {
                SenderId = 1,
                ReceiverId = 2,
                Text = faker.Lorem.Sentence()
            };

            var message = new Message
            {
                Id = 1,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Text = request.Text,
                CreatedAt = DateTime.UtcNow
            };

            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(x => x.Create(It.IsAny<Message>())).ReturnsAsync(1);
            var messageRepository = messageRepositoryMock.Object;
            var messageService = new MessageService(messageRepository, _mapper);

            // Act
            var result = await messageService.Create(request);

            // Assert
            result.Should().Be(1);
            messageRepositoryMock.Verify(x => x.Create(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task Update_ExistingMessage_ShouldUpdateMessage()
        {
            // Arrange
            var faker = new Faker();
            var request = new UpdateMessageRequest
            {
                Id = 1,
                Text = faker.Lorem.Sentence()
            };

            var existingMessage = new Message
            {
                Id = request.Id,
                Text = "Original text",
                SenderId = 1,
                ReceiverId = 2
            };

            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(x => x.GetById(request.Id)).ReturnsAsync(existingMessage);
            messageRepositoryMock.Setup(x => x.Update(It.IsAny<Message>())).ReturnsAsync(true);
            var messageRepository = messageRepositoryMock.Object;
            var messageService = new MessageService(messageRepository, _mapper);

            // Act
            await messageService.Update(request);

            // Assert
            messageRepositoryMock.Verify(x => x.GetById(request.Id), Times.Once);
            messageRepositoryMock.Verify(x => x.Update(It.Is<Message>(m => 
                m.Id == request.Id && m.Text == request.Text)), Times.Once);
        }

        [Fact]
        public async Task Delete_ExistingMessage_ShouldDeleteMessage()
        {
            // Arrange
            var messageId = 1;
            var existingMessage = new Message
            {
                Id = messageId,
                Text = "Hello!",
                SenderId = 1,
                ReceiverId = 2
            };

            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(x => x.GetById(messageId)).ReturnsAsync(existingMessage);
            messageRepositoryMock.Setup(x => x.Delete(messageId)).ReturnsAsync(true);
            var messageRepository = messageRepositoryMock.Object;
            var messageService = new MessageService(messageRepository, _mapper);

            // Act
            await messageService.Delete(messageId);

            // Assert
            messageRepositoryMock.Verify(x => x.GetById(messageId), Times.Once);
            messageRepositoryMock.Verify(x => x.Delete(messageId), Times.Once);
        }
    }
}
