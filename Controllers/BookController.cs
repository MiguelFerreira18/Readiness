using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using readiness.mapper;
using readiness.service;

namespace readiness.Controllers;

[ApiController]
[Route("api/books")]
[Authorize]
public class BookController : ControllerBase
{
    private readonly IBookService _service;
    public BookController(IBookService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create(CreateBookRequest request)
    {
        var userClaimId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(userClaimId, out var userId))
            return Unauthorized(new { message = "A valid authentication token is required." });

        string roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        if (!Enum.TryParse<Role>(roleClaim, ignoreCase: true, out var role) || role != Role.Administrator)
            return Forbid();
        
        var categories = new List<CategoryType>();
        if (request.Categories is not null)
        {
            foreach (var raw in request.Categories)
            {
                if (!Enum.TryParse<CategoryType>(raw, ignoreCase: true, out var parsed))
                    return null; // if category is not on list we ignore/remove it
                categories.Add(parsed);
            }
        }
        
        Book entityToSave = request.ToEntity();
        entityToSave.Categories = categories.Select(c => c.ToEntity()).ToList();
        
        var created = await _service.CreateAsync(request.ToEntity(), userId);
        if (created is null)
            return Conflict(new { message = "A book with this ISBN already exists." });

        return Created($"/api/books/{created.Id}", created);
    }
}