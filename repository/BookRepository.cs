using Microsoft.EntityFrameworkCore;


//TODO:DELETE ON MERGE SINCE THIS IS A MOCK UP
public interface IBookRepository
{
    public Task<Book> AddAsync(Book book);
    public Task<BookResponse> GetBookByIdAsync(long id);
}


public class BookRepository
{
    
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context) => _context = context;
    
    
    public Task<BookResponse?> GetBookByIdAsync(long id)
        => _context.Books
            .Where(b => b.Id == id)
            .ProjectToResponse()
            .FirstOrDefaultAsync();
    
    public async Task<Book> AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }
}