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
        var postId = await _postService.Create(request);
        return Created($"/post/{postId}", new { Id = postId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById( int id)
    {
        var post = await _postService.GetById(id);
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
        await _postService.Update(request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        await _postService.Delete(id);
        return NoContent();
    }
}

