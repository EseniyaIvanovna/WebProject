using Application.Exceptions;
using Application.Requests;
using Application.Service;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Services;

[Collection("IntegrationTests")]
public class InteractionServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IInteractionService _interactionService;

    public InteractionServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _interactionService = scope.ServiceProvider.GetRequiredService<IInteractionService>();
    }

    [Fact]
    public async Task GetAllInteractions_ShouldReturnInteractions()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var user2 = await _fixture.CreateUser();
        var request1 = new CreateInteractionRequest 
        { 
            Status = Status.Friend, 
            User1Id = user1.Id, 
            User2Id = user2.Id 
        };
        await _interactionService.Create(request1);

        var user3 = await _fixture.CreateUser();
        var request2 = new CreateInteractionRequest 
        { 
            Status = Status.Friend, 
            User1Id = user2.Id, 
            User2Id = user3.Id 
        };
        await _interactionService.Create(request2);

        // Act
        var interactions = (await _interactionService.GetAll()).ToList();

        // Assert
        interactions.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetInteractionById_ShouldReturnInteraction()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var user2 = await _fixture.CreateUser();
        var request = new CreateInteractionRequest 
        { 
            Status = Status.Friend, 
            User1Id = user1.Id, 
            User2Id = user2.Id 
        };
        var interactionId = await _interactionService.Create(request);

        // Act
        var interaction = await _interactionService.GetById(interactionId);

        // Assert
        interaction.Should().NotBeNull();
        interaction.Id.Should().Be(interactionId);
        interaction.Status.Should().Be(request.Status);
        interaction.User1Id.Should().Be(user1.Id);
        interaction.User2Id.Should().Be(user2.Id);
    }

    [Fact]
    public async Task GetInteractionsByStatus_ShouldReturnInteractions()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var user2 = await _fixture.CreateUser();
        var user3 = await _fixture.CreateUser();

        var request1 = new CreateInteractionRequest 
        { 
            Status = Status.Friend, 
            User1Id = user1.Id, 
            User2Id = user2.Id 
        };
        await _interactionService.Create(request1);

        var request2 = new CreateInteractionRequest 
        { 
            Status = Status.Friend, 
            User1Id = user1.Id, 
            User2Id = user3.Id 
        };
        await _interactionService.Create(request2);

        // Act
        var interactions = (await _interactionService.GetByStatus(Status.Friend)).ToList();

        // Assert
        interactions.Should().HaveCountGreaterThanOrEqualTo(2);
        interactions.Should().OnlyContain(i => i.Status == Status.Friend);
    }

    [Fact]
    public async Task CreateInteraction_ShouldCreateNewInteraction()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var user2 = await _fixture.CreateUser();
        var request = new CreateInteractionRequest 
        { 
            Status = Status.Friend, 
            User1Id = user1.Id, 
            User2Id = user2.Id 
        };

        // Act
        var interactionId = await _interactionService.Create(request);

        // Assert
        interactionId.Should().BeGreaterThan(0);
        var createdInteraction = await _interactionService.GetById(interactionId);
        createdInteraction.Should().NotBeNull();
        createdInteraction.Status.Should().Be(request.Status);
        createdInteraction.User1Id.Should().Be(user1.Id);
        createdInteraction.User2Id.Should().Be(user2.Id);
    }

    [Fact]
    public async Task DeleteInteraction_ShouldRemoveInteraction()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var user2 = await _fixture.CreateUser();
        var request = new CreateInteractionRequest 
        { 
            Status = Status.Friend, 
            User1Id = user1.Id, 
            User2Id = user2.Id 
        };
        var interactionId = await _interactionService.Create(request);

        // Act
        await _interactionService.Delete(interactionId);

        // Assert
        await _interactionService.Invoking(x => x.GetById(interactionId))
            .Should().ThrowAsync<NotFoundApplicationException>()
            .WithMessage($"Interaction {interactionId} not found");
    }
} 