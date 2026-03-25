using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TennisManager.API.Models.Requests;
using TennisManager.API.Models.Responses;
using TennisManager.Application.Common.Exceptions;
using TennisManager.Application.Common.Interfaces;
using TennisManager.Domain.Entities;
using TennisManager.Domain.Interfaces.Repositories;

namespace TennisManager.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public AuthController(ICurrentUserService currentUserService, IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Register a new user. The actual registration is handled by Supabase Auth.
    /// This endpoint creates the corresponding user profile in the application database.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            return Conflict(new { message = "User with this email already exists." });

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone
        };

        var created = await _userRepository.CreateAsync(user);

        return CreatedAtAction(nameof(GetCurrentUser), new UserResponse
        {
            Id = created.Id,
            Email = created.Email,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Phone = created.Phone,
            AvatarUrl = created.AvatarUrl,
            CreatedAt = created.CreatedAt
        });
    }

    /// <summary>
    /// Login placeholder – actual authentication is done via Supabase Auth.
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        return Ok(new { message = "Use Supabase Auth to obtain a JWT token." });
    }

    /// <summary>
    /// Get the profile of the currently authenticated user.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user is null)
            return NotFound(new { message = "User profile not found." });

        return Ok(new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        });
    }

    /// <summary>
    /// Update the profile of the currently authenticated user.
    /// </summary>
    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user is null)
            return NotFound(new { message = "User profile not found." });

        if (request.FirstName is not null) user.FirstName = request.FirstName;
        if (request.LastName is not null) user.LastName = request.LastName;
        if (request.Phone is not null) user.Phone = request.Phone;
        if (request.AvatarUrl is not null) user.AvatarUrl = request.AvatarUrl;

        await _userRepository.UpdateAsync(user);

        return Ok(new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        });
    }
}
