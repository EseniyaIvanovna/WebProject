using Application.Exceptions;
using Application.Requests;
using Application.Service;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Services;

[Collection("IntegrationTests")]
public class CommentServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly ICommentService _commentService;
    private readonly IPostService _postService;

    public CommentServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _commentService = scope.ServiceProvider.GetRequiredService<ICommentService>();
        _postService = scope.ServiceProvider.GetRequiredService<IPostService>();
    }

    [Fact]
    public async Task GetAllComments_ShouldReturnComments()
    {
        // Arrange
        var user1 = await _fixture.CreateUser();
        var post1 = await _fixture.CreatePost(user1.Id);
        var request1 = new CreateCommentRequest 
        { 
            Content = "Comment 1", 
            UserId = user1.Id, 
            PostId = post1.Id 
        };
        await _commentService.Create(request1);

        var user2 = await _fixture.CreateUser();
        var post2 = await _fixture.CreatePost(user2.Id);
        var request2 = new CreateCommentRequest 
        { 
            Content = "Comment 2", 
            UserId = user2.Id, 
            PostId = post2.Id 
        };
        await _commentService.Create(request2);

        // Act
        var comments = (await _commentService.GetAll()).ToList();

        // Assert
        comments.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetCommentById_ShouldReturnComment()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var request = new CreateCommentRequest 
        { 
            Content = "Test Comment", 
            UserId = user.Id, 
            PostId = post.Id 
        };
        var commentId = await _commentService.Create(request);

        // Act
        var comment = await _commentService.GetById(commentId);

        // Assert
        comment.Should().NotBeNull();
        comment.Id.Should().Be(commentId);
        comment.Content.Should().Be(request.Content);
        comment.UserId.Should().Be(user.Id);
        comment.PostId.Should().Be(post.Id);
    }

    [Fact]
    public async Task GetCommentsByUserId_ShouldReturnUserComments()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post1 = await _fixture.CreatePost(user.Id);
        var request1 = new CreateCommentRequest 
        { 
            Content = "Comment 1", 
            UserId = user.Id, 
            PostId = post1.Id 
        };
        await _commentService.Create(request1);

        var post2 = await _fixture.CreatePost(user.Id);
        var request2 = new CreateCommentRequest 
        { 
            Content = "Comment 2", 
            UserId = user.Id, 
            PostId = post2.Id 
        };
        await _commentService.Create(request2);

        // Act
        var comments = (await _commentService.GetByUserId(user.Id)).ToList();

        // Assert
        comments.Should().HaveCountGreaterThanOrEqualTo(2);
        comments.Should().OnlyContain(c => c.UserId == user.Id);
    }

    [Fact]
    public async Task CreateComment_ShouldCreateNewComment()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var request = new CreateCommentRequest 
        { 
            Content = "New Comment", 
            UserId = user.Id, 
            PostId = post.Id 
        };

        // Act
        var commentId = await _commentService.Create(request);

        // Assert
        commentId.Should().BeGreaterThan(0);
        var createdComment = await _commentService.GetById(commentId);
        createdComment.Should().NotBeNull();
        createdComment.Content.Should().Be(request.Content);
        createdComment.UserId.Should().Be(user.Id);
        createdComment.PostId.Should().Be(post.Id);
    }

    [Fact]
    public async Task UpdateComment_ShouldUpdateExistingComment()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var createRequest = new CreateCommentRequest 
        { 
            Content = "Original Content", 
            UserId = user.Id, 
            PostId = post.Id 
        };
        var commentId = await _commentService.Create(createRequest);

        var updateRequest = new UpdateCommentRequest 
        { 
            Id = commentId, 
            Content = "Updated Content" 
        };

        // Act
        await _commentService.Update(updateRequest);

        // Assert
        var updatedComment = await _commentService.GetById(commentId);
        updatedComment.Should().NotBeNull();
        updatedComment.Content.Should().Be(updateRequest.Content);
    }

    [Fact]
    public async Task DeleteComment_ShouldRemoveComment()
    {
        // Arrange
        var user = await _fixture.CreateUser();
        var post = await _fixture.CreatePost(user.Id);
        var request = new CreateCommentRequest 
        { 
            Content = "Comment to Delete", 
            UserId = user.Id, 
            PostId = post.Id 
        };
        var commentId = await _commentService.Create(request);

        // Act
        await _commentService.Delete(commentId);

        // Assert
        await _commentService.Invoking(x => x.GetById(commentId))
            .Should().ThrowAsync<NotFoundApplicationException>()
            .WithMessage($"Comment {commentId} not found");
    }
}
