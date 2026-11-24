//TODO:DELETE ON MERGE SINCE THIS IS A MOCK UP


public static class BookMapper
{
    public static BookResponse ToResponse(this Book book) => new(
        book.Id,
        book.Name,
        book.Summary,
        book.Publisher,
        book.YearOfRelease,
        book.NumberOfPage,
        book.Isbn,
        null); //TODO: Correct

    public static Book ToEntity(this CreateBookRequest request) => new()
    {
        Name = request.Name.Trim(),
        Summary = request.Summary?.Trim(),
        Publisher = request.Publisher.Trim(),
        YearOfRelease = request.YearOfRelease,
        NumberOfPage = request.NumberOfPage,
        Isbn = request.Isbn
    };

    
    public static IQueryable<BookResponse> ProjectToResponse(this IQueryable<Book> query)
        => query.Select(b => new BookResponse(
            b.Id,
            b.Name,
            b.Summary,
            b.Publisher,
            b.YearOfRelease,
            b.NumberOfPage,
            b.Isbn,
            null //TODO: Correct
        ));
}