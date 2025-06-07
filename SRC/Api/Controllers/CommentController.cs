using Application.Requests;
using Application.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
    {
        var commentId = await _commentService.Create(request);
        return Created($"/comment/{commentId}", new { Id = commentId });
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var comment = await _commentService.GetById(id);
        return Ok(comment);
    }

    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetCommentsByUserId(int userId)
    {
        var comments = await _commentService.GetByUserId(userId);
        return Ok(comments);
    }
    [HttpGet("ByPost/{postId}")]
    public async Task<IActionResult> GetCommentsByPostId(int postId)
    {
        var comments = await _commentService.GetByPostId(postId);
        return Ok(comments);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequest request)
    {
        await _commentService.Update(request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        await _commentService.Delete(id);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllComments()
    {
        var comments = await _commentService.GetAll();
        return Ok(comments);
    }
}

