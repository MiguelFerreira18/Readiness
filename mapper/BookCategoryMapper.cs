using readiness.dto;

namespace readiness.mapper;
public static class BookCategoryMapper
{
    public static BookCategoryResponse ToResponse(this BookCategory bc) => new(
        bc.Id,
        bc.BookId,
        bc.Category.ToString());

    public static BookCategory ToEntity(this CategoryType category, long bookId) => new()
    {
        BookId = bookId,
        Category = category
    };
    
    public static BookCategory ToEntity(this CategoryType category) => new()
    {
        Category = category
    };

    public static IQueryable<BookCategoryResponse> ProjectToResponse(
        this IQueryable<BookCategory> query)
        => query.Select(bc => new BookCategoryResponse(
            bc.Id,
            bc.BookId,
            bc.Category.ToString()));
}