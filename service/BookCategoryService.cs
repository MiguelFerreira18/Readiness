using readiness.dto;
using readiness.mapper;
using readiness.repository;

namespace readiness.service;

public interface IBookCategoryService
{
    Task<bool> AddCategoriesToBookAsync(long bookId, List<CategoryType> categories, long userId);
}

public class BookCategoryService : IBookCategoryService
{
    private readonly IBookCategoryRepository _bookCategoryRepository;
    private readonly IUserService _userService;

    public BookCategoryService(IUserService userService, IBookCategoryRepository bookCategoryRepository)
    {
        _userService = userService;
        _bookCategoryRepository = bookCategoryRepository;
    }

    public async Task<bool> AddCategoriesToBookAsync(
        long bookId, List<CategoryType> categories, long userId)
    {
        if (!await _userService.IsUserModerator(userId)) return false;
        if (categories is null || categories.Count == 0) return true;
        var entities = categories.Select(c => c.ToEntity(bookId)).ToList();
        await _bookCategoryRepository.AddRangeAsync(entities);
        return true;
    }

    public Task<List<BookCategoryResponse>> GetByBookIdAsync(long bookId)
    {
        return _bookCategoryRepository.GetResponsesByBookIdAsync(bookId);
    }
}