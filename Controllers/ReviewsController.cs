using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace readiness.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;
    public ReviewController(IReviewService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ReviewResponse>>> GetAll([FromQuery] ReviewFilter? filter)
    {
        var reviews = await _service.GetAllAsync(filter);
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponse>> GetById(long id)
    {
        var review = await _service.GetByIdAsync(id);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> Create(CreateReviewRequest request)
    {
        var userClaimId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(userClaimId, out var userId))
        {
            return Unauthorized(new { message = "A valid authentication token is required." });
        }
        var created = await _service.CreateAsync(request, userId);
        return created is null ? Conflict() : CreatedAtAction(nameof(GetById), new { id = created.id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewResponse>> Update(long id, UpdateReviewRequest request)
    {
        var userClaimId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(userClaimId, out var userId))
        {
            return Unauthorized(new { message = "A valid authentication token is required." });
        }
        var updated = await _service.UpdateAsync(userId, id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userClaimId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(userClaimId, out var userId))
        {
            return Unauthorized(new { message = "A valid authentication token is required." });
        }

        string roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        if (!Enum.TryParse<Role>(roleClaim, ignoreCase: true, out var role))
        {
            return Forbid();
        }
        bool isAdmin = role == Role.Moderator;
        var deleted = await _service.DeleteAsync(userId, id, isAdmin);
        return deleted ? NoContent() : NotFound();
    }
}
