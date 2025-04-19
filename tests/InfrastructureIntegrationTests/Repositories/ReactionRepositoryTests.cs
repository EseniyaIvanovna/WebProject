using Bogus;
using Domain;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureIntegrationTests.Repositories;

[Collection("IntegrationTests")]
public class ReactionRepositoryTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IReactionRepository _reactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly Faker _faker;

    public ReactionRepositoryTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _reactionRepository = scope.ServiceProvider.GetRequiredService<IReactionRepository>();
        _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();
        _faker = new Faker();
    }

    [Fact]
    public async Task Delete_WhenReactionExists_DeletesReaction()
    {
        // Arrange
        var user = await CreateUser();
        var post = await CreatePost(user.Id);
        var reaction = await CreateReaction(user.Id, post.Id);

        // Act
        var result = await _reactionRepository.Delete(reaction.Id);

        // Assert
        result.Should().BeTrue();
        var exists = await _reactionRepository.Exists(user.Id, post.Id);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_WhenReactionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentReactionId = 999999;

        // Act
        var result = await _reactionRepository.Delete(nonExistentReactionId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteByPostId_WhenReactionsExist_DeletesAllReactions()
    {
        // Arrange
        var user1 = await CreateUser();
        var user2 = await CreateUser();
        var post = await CreatePost(user1.Id);
        
        await CreateReaction(user1.Id, post.Id);
        await CreateReaction(user2.Id, post.Id);

        // Act
        await _reactionRepository.DeleteByPostId(post.Id);

        // Assert
        var exists1 = await _reactionRepository.Exists(user1.Id, post.Id);
        var exists2 = await _reactionRepository.Exists(user2.Id, post.Id);
        exists1.Should().BeFalse();
        exists2.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteByUserId_WhenReactionsExist_DeletesAllUserReactions()
    {
        // Arrange
        var user = await CreateUser();
        var post1 = await CreatePost(user.Id);
        var post2 = await CreatePost(user.Id);
        
        await CreateReaction(user.Id, post1.Id);
        await CreateReaction(user.Id, post2.Id);

        // Act
        await _reactionRepository.DeleteByUserId(user.Id);

        // Assert
        var exists1 = await _reactionRepository.Exists(user.Id, post1.Id);
        var exists2 = await _reactionRepository.Exists(user.Id, post2.Id);
        exists1.Should().BeFalse();
        exists2.Should().BeFalse();
    }

    [Fact]
    public async Task Exists_WhenReactionExists_ReturnsTrue()
    {
        // Arrange
        var user = await CreateUser();
        var post = await CreatePost(user.Id);
        await CreateReaction(user.Id, post.Id);

        // Act
        var exists = await _reactionRepository.Exists(user.Id, post.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Exists_WhenReactionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var user = await CreateUser();
        var post = await CreatePost(user.Id);

        // Act
        var exists = await _reactionRepository.Exists(user.Id, post.Id);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteByPostOwnerId_WhenReactionsExist_DeletesAllReactions()
    {
        // Arrange
        var postOwner = await CreateUser();
        var user1 = await CreateUser();
        var user2 = await CreateUser();
        var post1 = await CreatePost(postOwner.Id);
        var post2 = await CreatePost(postOwner.Id);
        
        await CreateReaction(user1.Id, post1.Id);
        await CreateReaction(user2.Id, post1.Id);
        await CreateReaction(user1.Id, post2.Id);
        await CreateReaction(user2.Id, post2.Id);

        // Act
        await _reactionRepository.DeleteByPostOwnerId(postOwner.Id);

        // Assert
        var exists1 = await _reactionRepository.Exists(user1.Id, post1.Id);
        var exists2 = await _reactionRepository.Exists(user2.Id, post1.Id);
        var exists3 = await _reactionRepository.Exists(user1.Id, post2.Id);
        var exists4 = await _reactionRepository.Exists(user2.Id, post2.Id);
        exists1.Should().BeFalse();
        exists2.Should().BeFalse();
        exists3.Should().BeFalse();
        exists4.Should().BeFalse();
    }

    private async Task<User> CreateUser()
    {
        var user = new User
        {
            Name = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            DateOfBirth = _faker.Date.Past(),
            Info = _faker.Lorem.Sentence(10),
            Email = _faker.Internet.Email()
        };
        user.Id = await _userRepository.Create(user);
        return user;
    }

    private async Task<Post> CreatePost(int userId)
    {
        var post = new Post
        {
            UserId = userId,
            Text = _faker.Lorem.Sentence(10),
            CreatedAt = DateTime.UtcNow
        };
        post.Id = await _postRepository.Create(post);
        return post;
    }

    private async Task<Reaction> CreateReaction(int userId, int postId)
    {
        var reaction = new Reaction
        {
            UserId = userId,
            PostId = postId,
            Type = ReactionType.Like,
        };
        reaction.Id = await _reactionRepository.Create(reaction);
        return reaction;
    }
}
