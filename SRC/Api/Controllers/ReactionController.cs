using Application.Requests;
using Application.Responses;
using Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
[ApiController]
[Route("[controller]")]
public class ReactionController:ControllerBase
{
    private readonly IReactionService _reactionService;

    public ReactionController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateReaction([FromBody] CreateReactionRequest request)
    {
        await _reactionService.Create(request);
        return Created();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReactionById(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var reaction = await _reactionService.GetById(id);
        if (reaction == null)
        {
            return NotFound();
        }

        return Ok(reaction);
    }
    
    [HttpGet("ByPost/{postId}")]
    public async Task<ActionResult<IEnumerable<ReactionResponse>>> GetReactionsByPostId(int postId)
    {
        if (postId < 0)
        {
            return BadRequest("Post ID must be a positive integer.");
        }

        var reactions = await _reactionService.GetByPostId(postId);
        return Ok(reactions);
    }
    
    [HttpGet("ByUser/{userId}")]
    public async Task<ActionResult<IEnumerable<ReactionResponse>>> GetReactionsByUserId(int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var reactions = await _reactionService.GetByUserId(userId);
        return Ok(reactions);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateReaction([FromBody] UpdateReactionRequest request)
    {
        var result = await _reactionService.Update(request);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReaction( int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await _reactionService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllReactions()
    {
        var reactions = await _reactionService.GetAll();
        return Ok(reactions);
    }
}

