using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace readiness.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IAuthService _service;

    public UsersController(IAuthService service) => _service = service;

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponse>> UpdateMyProfile(
        UpdateUserProfileRequest request)
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "A valid authentication token is required." });
        }

        var result = await _service.UpdateProfileAsync(userId, request);
        return result.Status switch
        {
            UserProfileUpdateStatus.Success => Ok(result.Profile),
            UserProfileUpdateStatus.UserNotFound => NotFound(new { message = "User not found." }),
            UserProfileUpdateStatus.EmailAlreadyInUse =>
                Conflict(new { message = "An account with this email already exists." }),
            _ => throw new InvalidOperationException("Unknown profile update result.")
        };
    }
}
