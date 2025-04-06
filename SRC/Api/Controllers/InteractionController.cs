using Application.Requests;
using Application.Service;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InteractionController : ControllerBase
{
    private readonly IInteractionService _interactionService;
 
    public InteractionController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInteraction([FromBody] CreateInteractionRequest request)
    {
        var interactionId = await _interactionService.Create(request);
        return Created($"/interaction/{interactionId}", new { Id = interactionId });
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInteractionById( int id)
    {
        var interaction = await _interactionService.GetById(id);
        return Ok(interaction);
    }
    
    [HttpGet("ByStatus/{status}")]
    public async Task<IActionResult> GetInteractionsByStatus(Status status)
    {
        var interactions = await _interactionService.GetByStatus(status);
        return Ok(interactions);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateInteraction([FromBody] UpdateInteractionRequest request)
    {
        await _interactionService.Update(request);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInteraction(int id)
    {
        await _interactionService.Delete(id);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllInteractions()
    {
        var interactions = await _interactionService.GetAll();
        return Ok(interactions);
    }
}

