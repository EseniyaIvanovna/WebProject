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
    private IUserService userService;
    public UserController(IUserService UserService)
    {
        userService = UserService;
    }
    //usercontroller 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromQuery]int Id)
    {
        if (Id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var user = await userService.GetById(Id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAll();
        return Ok(users);
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        var userId = await userService.Add(user);
        return Ok(userId);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        var result = await userService.Update(user);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromQuery] int Id)
    {
        if (Id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await userService.Delete(Id);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
