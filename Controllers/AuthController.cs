using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;

namespace readiness.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service) => _service = service;

    [HttpPost("register")]
    public async Task<ActionResult<RegisteredUserResponse>> Register(RegisterRequest request)
    {
        var registeredUser = await _service.RegisterAsync(request);
        return registeredUser is null
            ? Conflict(new { message = "An account with this email already exists." })
            : StatusCode(StatusCodes.Status201Created, registeredUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginUserResponse>> Login(LoginRequest request)
    {
        var login = await _service.LoginAsync(request);
        if (login is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        Response.Cookies.Append(
            JwtSettings.CookieName,
            login.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = login.ExpiresAtUtc,
                IsEssential = true,
                Path = "/"
            });

        return Ok(login.User);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var tokenId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expiration = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        if (string.IsNullOrWhiteSpace(tokenId) ||
            !long.TryParse(expiration, out var expirationUnixSeconds))
        {
            return Unauthorized(new { message = "A valid authentication token is required." });
        }

        await _service.RevokeTokenAsync(
            tokenId,
            DateTimeOffset.FromUnixTimeSeconds(expirationUnixSeconds).UtcDateTime);

        Response.Cookies.Delete(
            JwtSettings.CookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

        return NoContent();
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "A valid authentication token is required." });
        }

        var result = await _service.ChangePasswordAsync(userId, request);
        return result switch
        {
            ChangePasswordResult.Success => NoContent(),
            ChangePasswordResult.UserNotFound => NotFound(new { message = "User not found." }),
            ChangePasswordResult.InvalidCurrentPassword =>
                BadRequest(new { message = "The current password is incorrect." }),
            _ => throw new InvalidOperationException("Unknown password change result.")
        };
    }
}
