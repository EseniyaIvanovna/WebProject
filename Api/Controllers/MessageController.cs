using Application.Dto;
using Application.Service;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;
[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private IMessageService messageService;

    public MessageController(IMessageService MessageService)
    {
        messageService = MessageService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateMessage([FromBody] MessageDto message)
    {
        if (message == null)
        {
            return BadRequest("Message data is required.");
        }

        var messId = await messageService.Create(message);
        return Ok(messId);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMessageById([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var message = await messageService.GetById(id);
        if (message == null)
        {
            return NotFound();
        }

        return Ok(message);
    }
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetMessagesByUserId([FromQuery] int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var messages = await messageService.GetByUserId(userId);
        return Ok(messages);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllMessages()
    {
        var messages = await messageService.GetAll();
        return Ok(messages);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMessage([FromBody] MessageDto message)
    {
        if (message == null)
        {
            return BadRequest("Message data is required.");
        }

        var result = await messageService.Update(message);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await messageService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

}

