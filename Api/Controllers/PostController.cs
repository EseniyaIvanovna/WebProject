using Application.Dto;
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
    public async Task<IActionResult> CreatePost([FromBody] PostDto post)
    {
        if (post == null)
        {
            return BadRequest("Post data is required.");
        }

        var postId = await _postService.Create(post);
        return CreatedAtAction(nameof(GetPostById), new { id = postId }, post);
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
    public async Task<IActionResult> UpdatePost([FromBody] PostDto post)
    {
        if (post == null)
        {
            return BadRequest("Post data is required.");
        }

        var result = await _postService.Update(post);
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

