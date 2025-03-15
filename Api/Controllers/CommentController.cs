﻿using Application.Dto;
using Application.Service;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private ICommentService _commentService;
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CommentDto comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment data is required.");
        }

        var commentId = await _commentService.Create(comment);
        return CreatedAtAction(nameof(GetCommentById), new { id = commentId }, comment);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById([FromQuery] int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var comment = await _commentService.GetById(id);
        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }
    
    [HttpGet("ByUser/{userId}")]
    public async Task<IActionResult> GetCommentsByUserId(int userId)
    {
        if (userId < 0)
        {
            return BadRequest("User ID must be a positive integer.");
        }

        var comments = await _commentService.GetByUserId(userId);
        return Ok(comments);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateComment([FromBody] CommentDto comment)
    {
        if (comment == null)
        {
            return BadRequest("Comment data is required.");
        }

        var result = await _commentService.Update(comment);
        if (!result)
        {
            return NoContent();
        }

        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        if (id < 0)
        {
            return BadRequest("ID must be a positive integer.");
        }

        var result = await _commentService.Delete(id);
        if (!result)
        {
            return NoContent();
        }

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllComments()
    {
        var comments = await _commentService.GetAll();
        return Ok(comments);
    }
}

