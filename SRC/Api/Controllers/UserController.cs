using Api.Extensions;
using Application.Requests;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Application.Service.Interfaces;
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
        var user = await _userService.GetById(id);
        return Ok(user);
    }
    
    [HttpGet("userInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return NotFound();
        }
        var user = await _userService.GetById(userId.Value);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAll();
        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var userId = await _userService.Add(request);
        return Created($"/user/{userId}", new { Id = userId });
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var currentUserId = User.GetUserId();

        if (!User.IsInRole("Admin") && (currentUserId==null || currentUserId.Value != request.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _userService.Update(request);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.Delete(id);
        return NoContent();
    }
}
