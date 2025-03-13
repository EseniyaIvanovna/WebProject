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
    private IReactionService reactionService;

    public ReactionController(IReactionService ReactionService)
    {
        reactionService = ReactionService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateReaction([FromBody] ReactionDto reaction)
    {
        if (reaction == null)
        {
            return BadRequest("Reaction data is required.");
        }

        var result = await reactionService.Create(reaction);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReactionById([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var reaction = await reactionService.GetById(id);
        if (reaction == null)
        {
            return NotFound();
        }

        return Ok(reaction);
    }
    [HttpGet("ByPost/{postId}")]
    public async Task<ActionResult<IEnumerable<ReactionDto>>> GetReactionsByPostId([FromQuery] int postId)
    {
        if (postId < 0)
        {
            return BadRequest("Post ID must be a positive integer.");
        }

        var reactions = await reactionService.GetByPostId(postId);
        return Ok(reactions);
    }
    [HttpGet("ByUser/{userId}")]
    public async Task<ActionResult<IEnumerable<ReactionDto>>> GetReactionsByUserId([FromQuery] int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var reactions = await reactionService.GetByUserId(userId);
        return Ok(reactions);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateReaction([FromBody] ReactionDto reaction)
    {
        if (reaction == null)
        {
            return BadRequest("Reaction data is required.");
        }

        var result = await reactionService.Update(reaction);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReaction([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await reactionService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllReactions()
    {
        var reactions = await reactionService.GetAll();
        return Ok(reactions);
    }


}

