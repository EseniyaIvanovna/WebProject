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
    public class InteractionServiceTests
    {
        private readonly Mock<IInteractionRepository> _interactionRepositoryMock;
        private readonly IMapper _mapper;
        private readonly IInteractionService _interactionService;
        private readonly Interaction _testInteraction;
        private readonly Mock<ILogger<InteractionService>> _loggerMock;
        private readonly Faker _faker;

        public InteractionServiceTests()
        {
            _faker = new Faker();

            _testInteraction = new Interaction
            {
                Id = 1,
                User1Id = 1,
                User2Id = 2,
                Status = Status.Friend
            };

            _interactionRepositoryMock = new Mock<IInteractionRepository>();
            _interactionRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(_testInteraction);

            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mappingConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<InteractionService>>();

            _interactionService = new InteractionService(_interactionRepositoryMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public void ShouldBeAvailableToCreate()
        {
            // Assert
            _interactionService.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ExistingInteraction_ShouldReturnInteractionResponse()
        {
            // Arrange
            _interactionRepositoryMock.Setup(x => x.GetById(_testInteraction.Id))
                .ReturnsAsync(_testInteraction);

            // Act
            var result = await _interactionService.GetById(_testInteraction.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_testInteraction.Id);
            result.User1Id.Should().Be(_testInteraction.User1Id);
            result.User2Id.Should().Be(_testInteraction.User2Id);
            result.Status.Should().Be(_testInteraction.Status);
            _interactionRepositoryMock.Verify(x => x.GetById(_testInteraction.Id), Times.Once);
        }

        [Fact]
        public async Task GetById_NonExistingInteraction_ShouldThrowNotFoundApplicationException()
        {
            // Arrange
            var nonExistingId = 999;
            _interactionRepositoryMock.Setup(x => x.GetById(nonExistingId))
                .ReturnsAsync((Interaction?)null);

            // Act & Assert
            await _interactionService.Invoking(x => x.GetById(nonExistingId))
                .Should().ThrowAsync<NotFoundApplicationException>()
                .WithMessage($"Interaction {nonExistingId} not found");
        }

        [Fact]
        public async Task Create_ValidRequest_ShouldCreateInteraction()
        {
            // Arrange
            var request = new CreateInteractionRequest
            {
                User1Id = _testInteraction.User1Id,
                User2Id = _testInteraction.User2Id,
                Status = _testInteraction.Status
            };

            _interactionRepositoryMock.Setup(x => x.ExistsBetweenUsers(request.User1Id, request.User2Id))
                .ReturnsAsync(false);
            _interactionRepositoryMock.Setup(x => x.Create(It.IsAny<Interaction>()))
                .ReturnsAsync(_testInteraction.Id);

            // Act
            var result = await _interactionService.Create(request);

            // Assert
            result.Should().Be(_testInteraction.Id);
            _interactionRepositoryMock.Verify(x => x.Create(It.Is<Interaction>(i =>
                i.User1Id == request.User1Id &&
                i.User2Id == request.User2Id &&
                i.Status == request.Status)), Times.Once);
        }

        [Fact]
        public async Task Create_WhenInteractionExists_ShouldThrowConflictException()
        {
            // Arrange
            var request = new CreateInteractionRequest
            {
                User1Id = _testInteraction.User1Id,
                User2Id = _testInteraction.User2Id,
                Status = _testInteraction.Status
            };

            _interactionRepositoryMock.Setup(x => x.ExistsBetweenUsers(request.User1Id, request.User2Id))
                .ReturnsAsync(true);

            // Act & Assert
            await _interactionService.Invoking(x => x.Create(request))
                .Should().ThrowAsync<ConflictApplicationException>()
                .WithMessage("Interaction between these users already exists");
        }

        [Fact]
        public async Task Update_ExistingInteraction_ShouldUpdateInteraction()
        {
            // Arrange
            var request = new UpdateInteractionRequest
            {
                Id = _testInteraction.Id,
                Status = Status.Blocked
            };

            _interactionRepositoryMock.Setup(x => x.GetById(_testInteraction.Id))
                .ReturnsAsync(_testInteraction);
            _interactionRepositoryMock.Setup(x => x.Update(It.IsAny<Interaction>()))
                .ReturnsAsync(true);

            // Act
            await _interactionService.Update(request);

            // Assert
            _interactionRepositoryMock.Verify(x => x.GetById(_testInteraction.Id), Times.Once);
            _interactionRepositoryMock.Verify(x => x.Update(It.Is<Interaction>(i =>
                i.Id == request.Id &&
                i.Status == request.Status)), Times.Once);
        }

        [Fact]
        public async Task Delete_ExistingInteraction_ShouldDeleteInteraction()
        {
            // Arrange
            _interactionRepositoryMock.Setup(x => x.GetById(_testInteraction.Id))
                .ReturnsAsync(_testInteraction);
            _interactionRepositoryMock.Setup(x => x.Delete(_testInteraction.Id))
                .ReturnsAsync(true);

            // Act
            await _interactionService.Delete(_testInteraction.Id);

            // Assert
            _interactionRepositoryMock.Verify(x => x.GetById(_testInteraction.Id), Times.Once);
            _interactionRepositoryMock.Verify(x => x.Delete(_testInteraction.Id), Times.Once);
        }

        [Fact]
        public async Task GetByStatus_ShouldReturnInteractions()
        {
            // Arrange
            var status = Status.Friend;
            var interactions = new List<Interaction> { _testInteraction };
            _interactionRepositoryMock.Setup(x => x.GetByStatus(status))
                .ReturnsAsync(interactions);

            // Act
            var result = await _interactionService.GetByStatus(status);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            var firstInteraction = result.First();
            firstInteraction.Id.Should().Be(_testInteraction.Id);
            firstInteraction.User1Id.Should().Be(_testInteraction.User1Id);
            firstInteraction.User2Id.Should().Be(_testInteraction.User2Id);
            firstInteraction.Status.Should().Be(_testInteraction.Status);
            _interactionRepositoryMock.Verify(x => x.GetByStatus(status), Times.Once);
        }

        [Fact]
        public async Task GetAll_NoInteractions_ShouldReturnEmptyList()
        {
            // Arrange
            _interactionRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(new List<Interaction>());

            // Act
            var result = await _interactionService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _interactionRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
