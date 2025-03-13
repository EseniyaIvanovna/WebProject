using Application.Dto;
using Application.Service;
using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private IPostService postService;
    public PostController(IPostService PostService)
    {
        postService = PostService;
    }


    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostDto post)
    {
        if (post == null)
        {
            return BadRequest("Post data is required.");
        }

        var postId = await postService.Create(post);
        return Ok(postId);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var post = await postService.GetById(id);
        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await postService.GetAll();
        return Ok(posts);
    }
    [HttpPut]
    public async Task<IActionResult> UpdatePost([FromBody] PostDto post)
    {
        if (post == null)
        {
            return BadRequest("Post data is required.");
        }

        var result = await postService.Update(post);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await postService.Delete(id);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }
}

