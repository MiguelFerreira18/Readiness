using Microsoft.EntityFrameworkCore;
using readiness.dto;
using readiness.mapper;

namespace readiness.repository;

public interface IBookCategoryRepository
{
    Task AddRangeAsync(List<BookCategory> bookCategories);
    Task<List<BookCategory>> GetByBookIdAsync(long bookId);
    Task<List<BookCategoryResponse>> GetResponsesByBookIdAsync(long bookId);
    Task SaveAsync();
}

public class BookCategoryRepository : IBookCategoryRepository
{
    private readonly AppDbContext _context;

    public BookCategoryRepository(AppDbContext context) => _context = context;

    public async Task AddRangeAsync(List<BookCategory> categories)
    {
        await _context.BookCategories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }
    
    public Task<List<BookCategory>> GetByBookIdAsync(long bookId)
        => _context.BookCategories
            .Where(bc => bc.BookId == bookId)
            .ToListAsync();

    public Task<List<BookCategoryResponse>> GetResponsesByBookIdAsync(long bookId)
        => _context.BookCategories
            .Where(bc => bc.BookId == bookId)
            .ProjectToResponse()
            .ToListAsync();
    
    public Task SaveAsync() => _context.SaveChangesAsync();
}