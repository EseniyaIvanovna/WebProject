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
        await _interactionService.Create(request);
        return Created();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInteractionById( int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var interaction = await _interactionService.GetById(id);
        if (interaction == null)
        {
            return NotFound();
        }

        return Ok(interaction);
    }
    
    [HttpGet("ByStatus/{status}")]
    public async Task<IActionResult> GetInteractionsByStatus(Status status)
    {
        if (!Enum.IsDefined(typeof(Status), status))
        {
            return BadRequest("Invalid status value.");
        }

        var interactions = await _interactionService.GetByStatus(status);
        return Ok(interactions);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateInteraction([FromBody] UpdateInteractionRequest request)
    {
        var result = await _interactionService.Update(request);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInteraction(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await _interactionService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllInteractions()
    {
        var interactions = await _interactionService.GetAll();
        return Ok(interactions);
    }
}

