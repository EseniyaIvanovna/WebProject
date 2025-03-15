using Application.Dto;
using Application.Service;
using Domain;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InteractionController : ControllerBase
{
    private IInteractionService _interactionService;
 
    public InteractionController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInteraction([FromBody] InteractionDto interaction)
    {
        if (interaction == null)
        {
            return BadRequest("Interaction data is required.");
        }

        var interactionId = await _interactionService.Create(interaction);
        return CreatedAtAction(nameof(GetInteractionById), new { id = interactionId }, interaction);
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
            return NoContent();
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
    public async Task<IActionResult> UpdateInteraction([FromBody] InteractionDto interaction)
    {
        if (interaction == null)
        {
            return BadRequest("Interaction data is required.");
        }

        var result = await _interactionService.Update(interaction);
        if (!result)
        {
            return NoContent();
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
            return NoContent();
        }

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllInteractions()
    {
        var interactions = await _interactionService.GetAll();
        return Ok(interactions);
    }
}

