using Application.Requests;
using Application.Service;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> CreateMessage([FromBody] CreateMessageRequest request)
    {
        await _messageService.Create(request);
        return Created();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMessageById(int id)
    {
        var message = await _messageService.GetById(id);
        return Ok(message);
    }
    
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetMessagesByUserId(int userId)
    {
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
    public async Task<IActionResult> UpdateMessage([FromBody] UpdateMessageRequest request)
    {
        await _messageService.Update(request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        await _messageService.Delete(id);
        return NoContent();
    }
}

