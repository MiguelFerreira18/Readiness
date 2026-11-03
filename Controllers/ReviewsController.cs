using Microsoft.AspNetCore.Mvc;

namespace readiness.Controllers;

[ApiController]
[Route("api/reviews")]
//TODO: ADD AUTHORIZE AFTER JWT IMPLEMENTATION
public class ReviewController : ControllerBase
{
    private readonly IReviewService _service;
    public ReviewController(IReviewService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ReviewResponse>>> GetAll()
    {
        var reviews = await _service.GetAllAsync();
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
        var created = await _service.CreateAsync(request, 1L);
        return created is null ? Conflict() : CreatedAtAction(nameof(GetById), new { id = created.id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewResponse>> Update(long id, UpdateReviewRequest request)
    {
        var updated = await _service.UpdateAsync(1L, id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        //TODO: BOTH 1 AND true WILL CHANGE AFTER JWT IMPLEMENTATION
        var deleted = await _service.DeleteAsync(1L, id, true);
        return deleted ? NoContent() : NotFound();
    }
}
