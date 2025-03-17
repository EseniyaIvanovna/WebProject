using Application.Dto;
using Application.Service;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
    public async Task<IActionResult> CreateUser([FromBody] UserDto user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }
        try
        {
            var userId = await _userService.Add(user);
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, user); // 201 Created
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message); // 409 Conflict, если пользователь уже существует
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message); // 400 Bad Request, если данные невалидны
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        try
        {
            var result = await _userService.Update(user);
            if (!result)
            {
                return NotFound(); 
            }
            return Ok(result); 
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message); 
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message); 
        }
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
