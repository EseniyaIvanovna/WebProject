using Application.Requests;
using Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var user = await _userService.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
   
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAll();
        return Ok(users);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
         await _userService.Add(request);
        return Created();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var result = await _userService.Update(request);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        try
        {
            var result = await _userService.Delete(id);
            if (!result)
            {
                return NotFound(); 
            }
            return NoContent(); 
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message); 
        }
    }
}
