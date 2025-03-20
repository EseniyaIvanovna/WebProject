using Application.Dto;
using Application.Service;
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
    public async Task<IActionResult> CreateComment([FromBody] CommentDto comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment data is required.");
        }

        var commentId = await _commentService.Create(comment);
        return CreatedAtAction(nameof(GetCommentById), new { id = commentId }, comment);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById( int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var comment = await _commentService.GetById(id);
        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }
    
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetCommentsByUserId(int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var comments = await _commentService.GetByUserId(userId);
        return Ok(comments);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateComment([FromBody] CommentDto comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment data is required.");
        }

        var result = await _commentService.Update(comment);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await _commentService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllComments()
    {
        var comments = await _commentService.GetAll();
        return Ok(comments);
    }
}

