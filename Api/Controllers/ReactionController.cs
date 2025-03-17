using Application.Dto;
using Application.Service;
using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
    public async Task<IActionResult> CreateReaction([FromBody] ReactionDto reaction)
    {
        if (reaction == null)
        {
            return BadRequest("Reaction data is required.");
        }

        var reactionId = await _reactionService.Create(reaction);
        return CreatedAtAction(nameof(GetReactionById), new { id = reactionId }, reaction);
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
    public async Task<ActionResult<IEnumerable<ReactionDto>>> GetReactionsByPostId(int postId)
    {
        if (postId < 0)
        {
            return BadRequest("Post ID must be a positive integer.");
        }

        var reactions = await _reactionService.GetByPostId(postId);
        return Ok(reactions);
    }
    
    [HttpGet("ByUser/{userId}")]
    public async Task<ActionResult<IEnumerable<ReactionDto>>> GetReactionsByUserId(int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var reactions = await _reactionService.GetByUserId(userId);
        return Ok(reactions);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateReaction([FromBody] ReactionDto reaction)
    {
        if (reaction == null)
        {
            return BadRequest("Reaction data is required.");
        }

        var result = await _reactionService.Update(reaction);
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

