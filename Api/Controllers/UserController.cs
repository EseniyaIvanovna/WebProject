using Application.Dto;
using Application.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IUserService userService;
    public UserController(IUserService userService)
    {
        this.userService = userService;
    }
    //usercontroller 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery]int Id)
    {
        var user = await userService.GetById(Id);
        return Ok(user);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await userService.GetAll();
        return Ok(users);
    }
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] UserDto user)
    {
        await userService.Add(user);
        return Created();
    }
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserDto user)
    {
        var result = await userService.Update(user);
        return Ok(result);
    }
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int Id)
    {
        var result = await userService.Delete(Id);
        return Ok(result);
    }
}
