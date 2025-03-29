using Application.Requests;
using Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        await _postService.Create(request);
        return Created();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById( int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var post = await _postService.GetById(id);
        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postService.GetAll();
        return Ok(posts);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePost([FromBody]UpdatePostRequest request)
    {
        var result = await _postService.Update(request);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await _postService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

