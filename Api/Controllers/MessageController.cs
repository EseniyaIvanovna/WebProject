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
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateMessage([FromBody] MessageDto message)
    {
        if (message == null)
        {
            return BadRequest("Message data is required.");
        }

        var messageId = await _messageService.Create(message);
        return CreatedAtAction(nameof(GetMessageById), new { id = messageId }, message);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMessageById(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var message = await _messageService.GetById(id);
        if (message == null)
        {
            return NotFound();
        }

        return Ok(message);
    }
    
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetMessagesByUserId(int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var messages = await _messageService.GetByUserId(userId);
        return Ok(messages);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllMessages()
    {
        var messages = await _messageService.GetAll();
        return Ok(messages);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMessage([FromBody] MessageDto message)
    {
        if (message == null)
        {
            return BadRequest("Message data is required.");
        }

        var result = await _messageService.Update(message);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await _messageService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

