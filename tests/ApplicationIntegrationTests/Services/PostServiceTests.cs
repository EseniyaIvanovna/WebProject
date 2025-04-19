using Application.Exceptions;
using Application.Requests;
using Application.Service;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Services;

[Collection("IntegrationTests")]
public class PostServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IPostService _postService;
    private readonly Faker _faker;

    public PostServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _postService = scope.ServiceProvider.GetRequiredService<IPostService>();
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAll_WhenPostsExist_ReturnsPosts()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var request1 = new CreatePostRequest 
        { 
            Text = "Post 1 Content", 
            UserId = user1.Id 
        };
        await _postService.Create(request1);

        var user2 = await _fixture.CreateUser();
        var request2 = new CreatePostRequest 
        { 
            Text = "Post 2 Content", 
            UserId = user2.Id 
        };
        await _postService.Create(request2);

        // Act
        var posts = (await _postService.GetAll()).ToList();

        // Assert
        posts.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_WhenPostExists_ReturnsPost()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var request = new CreatePostRequest 
        { 
            Text = "Test Content", 
            UserId = user.Id 
        };
        var postId = await _postService.Create(request);

        // Act
        var post = await _postService.GetById(postId);

        // Assert
        post.Should().NotBeNull();
        post.Id.Should().Be(postId);
        post.Text.Should().Be(request.Text);
        post.UserId.Should().Be(user.Id);
        post.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task Create_WhenValidRequest_CreatesPost()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var request = new CreatePostRequest
        {
            UserId = user.Id,
            Text = _faker.Lorem.Sentence(10)
        };

        // Act
        var postId = await _postService.Create(request);

        // Assert
        postId.Should().BeGreaterThan(0);
        var createdPost = await _postService.GetById(postId);
        createdPost.Should().NotBeNull();
        createdPost.Text.Should().Be(request.Text);
        createdPost.UserId.Should().Be(user.Id);
        createdPost.CreatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task Update_WhenPostExists_UpdatesPost()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var createRequest = new CreatePostRequest 
        { 
            Text = "Original Content", 
            UserId = user.Id 
        };
        var postId = await _postService.Create(createRequest);

        var updateRequest = new UpdatePostRequest 
        { 
            Id = postId, 
            Text = _faker.Lorem.Sentence(10)
        };

        // Act
        await _postService.Update(updateRequest);

        // Assert
        var updatedPost = await _postService.GetById(postId);
        updatedPost.Should().NotBeNull();
        updatedPost.Text.Should().Be(updateRequest.Text);
    }

    [Fact]
    public async Task GetById_WhenPostDoesNotExist_ThrowsException()
    {
        // Arrange
        var nonExistentPostId = 999999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundApplicationException>(() => _postService.GetById(nonExistentPostId));
    }

    [Fact]
    public async Task Update_WhenPostDoesNotExist_ThrowsException()
    {
        // Arrange
        var updateRequest = new UpdatePostRequest 
        { 
            Id = 999999,
            Text = "Updated Content"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundApplicationException>(() => _postService.Update(updateRequest));
    }

    [Fact]
    public async Task Create_WhenUserIdIsInvalid_ThrowsException()
    {
        // Arrange
        var request = new CreatePostRequest
        {
            UserId = 999999,
            Text = _faker.Lorem.Sentence(10)
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundApplicationException>(() => _postService.Create(request));
    }
}
