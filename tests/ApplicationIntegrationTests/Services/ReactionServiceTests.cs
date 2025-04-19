using Application.Exceptions;
using Application.Requests;
using Application.Service;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Services;

[Collection("IntegrationTests")]
public class ReactionServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IReactionService _reactionService;
    private readonly IPostService _postService;

    public ReactionServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _reactionService = scope.ServiceProvider.GetRequiredService<IReactionService>();
        _postService = scope.ServiceProvider.GetRequiredService<IPostService>();
    }

    [Fact]
    public async Task GetAllReactions_ShouldReturnReactions()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var post1 = await _fixture.CreatePost(user1.Id);
        var request1 = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user1.Id, 
            PostId = post1.Id 
        };
        await _reactionService.Create(request1);

        var user2 = await _fixture.CreateUser();
        var post2 = await _fixture.CreatePost(user2.Id);
        var request2 = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user2.Id, 
            PostId = post2.Id 
        };
        await _reactionService.Create(request2);

        // Act
        var reactions = (await _reactionService.GetAll()).ToList();

        // Assert
        reactions.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetReactionById_ShouldReturnReaction()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var request = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user.Id, 
            PostId = post.Id 
        };
        var reactionId = await _reactionService.Create(request);

        // Act
        var reaction = await _reactionService.GetById(reactionId);

        // Assert
        reaction.Should().NotBeNull();
        reaction.Id.Should().Be(reactionId);
        reaction.Type.Should().Be(request.Type.ToString());
        reaction.UserId.Should().Be(user.Id);
        reaction.PostId.Should().Be(post.Id);
    }

    [Fact]
    public async Task GetReactionsByUserId_ShouldReturnUserReactions()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post1 = await _fixture.CreatePost(user.Id);
        var post2 = await _fixture.CreatePost(user.Id);

        var request1 = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user.Id, 
            PostId = post1.Id 
        };
        await _reactionService.Create(request1);

        var request2 = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user.Id, 
            PostId = post2.Id 
        };
        await _reactionService.Create(request2);

        // Act
        var reactions = (await _reactionService.GetByUserId(user.Id)).ToList();

        // Assert
        reactions.Should().HaveCountGreaterThanOrEqualTo(2);
        reactions.Should().OnlyContain(r => r.UserId == user.Id);
    }

    [Fact]
    public async Task GetReactionsByPostId_ShouldReturnPostReactions()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var user2 = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user1.Id);

        var request1 = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user1.Id, 
            PostId = post.Id 
        };
        await _reactionService.Create(request1);

        var request2 = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user2.Id, 
            PostId = post.Id 
        };
        await _reactionService.Create(request2);

        // Act
        var reactions = (await _reactionService.GetByPostId(post.Id)).ToList();

        // Assert
        reactions.Should().HaveCountGreaterThanOrEqualTo(2);
        reactions.Should().OnlyContain(r => r.PostId == post.Id);
    }

    [Fact]
    public async Task CreateReaction_ShouldCreateNewReaction()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var request = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user.Id, 
            PostId = post.Id 
        };

        // Act
        var reactionId = await _reactionService.Create(request);

        // Assert
        reactionId.Should().BeGreaterThan(0);
        var createdReaction = await _reactionService.GetById(reactionId);
        createdReaction.Should().NotBeNull();
        createdReaction.Type.Should().Be(request.Type.ToString());
        createdReaction.UserId.Should().Be(user.Id);
        createdReaction.PostId.Should().Be(post.Id);
    }

    [Fact]
    public async Task UpdateReaction_ShouldUpdateExistingReaction()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var createRequest = new CreateReactionRequest 
        { 
            Type = Domain.Enums.ReactionType.Like, 
            UserId = user.Id, 
            PostId = post.Id 
        };
        var reactionId = await _reactionService.Create(createRequest);

        var updateRequest = new UpdateReactionRequest 
        { 
            Id = reactionId, 
            Type = Domain.Enums.ReactionType.Dislike 
        };

        // Act
        await _reactionService.Update(updateRequest);

        // Assert
        var updatedReaction = await _reactionService.GetById(reactionId);
        updatedReaction.Should().NotBeNull();
        updatedReaction.Type.Should().Be(updateRequest.Type.ToString());
    }

    [Fact]
    public async Task DeleteReaction_ShouldRemoveReaction()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var request = new CreateReactionRequest 
        { 
            Type = ReactionType.Like, 
            UserId = user.Id, 
            PostId = post.Id 
        };
        var reactionId = await _reactionService.Create(request);

        // Act
        await _reactionService.Delete(reactionId);

        // Assert
        await _reactionService.Invoking(x => x.GetById(reactionId))
            .Should().ThrowAsync<NotFoundApplicationException>()
            .WithMessage($"Reaction {reactionId} not found");
    }
}
