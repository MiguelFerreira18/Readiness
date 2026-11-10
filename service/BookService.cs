using readiness.repository;

namespace readiness.service;


public interface IBookService
{
    Task<BookResponse?> CreateAsync(CreateBookRequest request, long userId);
}


public class BookService : IBookService
{
    private readonly IUserService _userService;
    private readonly IBookRepository _bookRepository;

    public BookService(IUserService userService, IBookRepository bookRepository)
    {
        _userService = userService;
        _bookRepository = bookRepository;
    }

    public async Task<BookResponse?> CreateAsync(CreateBookRequest request, long userId)
    {
        if (!await _userService.IsUserModerator(userId)) return null;
        
        Book newBook = request.ToEntity();
        await _bookRepository.AddAsync(newBook);
        return (await _bookRepository.GetBookByIdAsync(newBook.Id));
    }
}