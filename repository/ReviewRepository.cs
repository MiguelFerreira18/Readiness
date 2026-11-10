using Microsoft.EntityFrameworkCore;

public interface IReviewRepository
{
    public Task<List<ReviewResponse>> GetAllReviewEsponseAsync();
    public Task<ReviewResponse?> GetReviewResponseByIdAsync(long id);
    public Task<Review?> GetReviewByBookIdAndUserIdAsync(long bookId, long userId);
    public Task<Review?> GetByIdAsync(long id);
    public Task<Review> AddAsync(Review review);
    public Task SaveAsync();
}


public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;
    public ReviewRepository(AppDbContext context) => _context = context;

    public Task<List<ReviewResponse>> GetAllReviewEsponseAsync()
    => _context.Reviews
    .ProjectToResponse()
    .AsNoTracking()
    .ToListAsync();

    public Task<ReviewResponse?> GetReviewResponseByIdAsync(long id)
    => _context.Reviews
    .Where(r => r.Id == id)
    .ProjectToResponse()
    .FirstOrDefaultAsync();

    public Task<Review?> GetReviewByBookIdAndUserIdAsync(long bookId, long userId)
    => _context.Reviews
    .Where(r => r.BookId == bookId && r.UserId == userId)
    .FirstOrDefaultAsync();

    public Task<Review?> GetByIdAsync(long id)
    => _context.Reviews
    .Include(r => r.Book)
    .Include(r => r.User)
    .Where(r => r.Id == id)
    .FirstOrDefaultAsync();

    public async Task<Review> AddAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public Task SaveAsync() => _context.SaveChangesAsync();

    public async Task<bool> DeleteAsync(long id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review is null) return false;

        _context.Reviews.Remove(review);
        return await _context.SaveChangesAsync() > 0;
    }

}

