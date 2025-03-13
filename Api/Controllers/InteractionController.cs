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
    private IInteractionService interactionService;
    public InteractionController(IInteractionService InteractionService)
    {
        interactionService = InteractionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInteraction([FromBody] InteractionDto interaction)
    {
        if (interaction == null)
        {
            return BadRequest("Interaction data is required.");
        }

        var interactionId = await interactionService.Create(interaction);
        return Ok(interactionId);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInteractionById( [FromQuery]int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var interaction = await interactionService.GetById(id);
        if (interaction == null)
        {
            return NotFound();
        }

        return Ok(interaction);
    }
    [HttpGet("ByStatus/{status}")]
    public async Task<IActionResult> GetInteractionsByStatus([FromQuery]Status status)
    {
        if (!Enum.IsDefined(typeof(Status), status))
        {
            return BadRequest("Invalid status value.");
        }

        var interactions = await interactionService.GetByStatus(status);
        return Ok(interactions);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateInteraction([FromBody] InteractionDto interaction)
    {
        if (interaction == null)
        {
            return BadRequest("Interaction data is required.");
        }

        var result = await interactionService.Update(interaction);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInteraction([FromQuery]int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await interactionService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllInteractions()
    {
        var interactions = await interactionService.GetAll();
        return Ok(interactions);
    }

}

