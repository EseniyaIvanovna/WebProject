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
        var reaction = await _reactionService.GetById(id);
        return Ok(reaction);
    }
    
    [HttpGet("ByPost/{postId}")]
    public async Task<ActionResult<IEnumerable<ReactionResponse>>> GetReactionsByPostId(int postId)
    {
        var reactions = await _reactionService.GetByPostId(postId);
        return Ok(reactions);
    }
    
    [HttpGet("ByUser/{userId}")]
    public async Task<ActionResult<IEnumerable<ReactionResponse>>> GetReactionsByUserId(int userId)
    {
        var reactions = await _reactionService.GetByUserId(userId);
        return Ok(reactions);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateReaction([FromBody] UpdateReactionRequest request)
    {
        await _reactionService.Update(request);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReaction( int id)
    {
        await _reactionService.Delete(id);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllReactions()
    {
        var reactions = await _reactionService.GetAll();
        return Ok(reactions);
    }
}

