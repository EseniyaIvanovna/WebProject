using Application.Dto;
using Application.Service;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private ICommentService commentService;
    public CommentController(ICommentService CommentService)
    {
        commentService = CommentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CommentDto comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment data is required.");
        }

        var commentId = await commentService.Create(comment);
        return Ok(commentId);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var comment = await commentService.GetById(id);
        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetCommentsByUserId([FromQuery]int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var comments = await commentService.GetByUserId(userId);
        return Ok(comments);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment([FromBody] CommentDto comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment data is required.");
        }

        var result = await commentService.Update(comment);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment([FromQuery]int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await commentService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllComments()
    {
        var comments = await commentService.GetAll();
        return Ok(comments);
    }

}

