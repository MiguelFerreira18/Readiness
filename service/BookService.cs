using readiness.mapper;
using readiness.repository;

namespace readiness.service;


public interface IBookService
{
    Task<BookResponse?> CreateAsync(Book book, long userId);
}


public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    
    public async Task<BookResponse?> CreateAsync(Book book, long userId)
    {
        if (await _bookRepository.ExistsByIsbnAsync(book.Isbn))    return null;
        await _bookRepository.AddAsync(book);
        return await _bookRepository.GetBookByIdAsync(book.Id);
    }
}