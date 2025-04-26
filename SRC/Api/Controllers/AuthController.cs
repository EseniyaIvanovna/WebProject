﻿using Application.Requests;
using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [EnableRateLimiting("login")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegistrationRequest request)
        {
            await authService.Register(request);
            return Created();
        }

        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var response = await authService.Login(request);
            return Ok(response);
        }

    }
}