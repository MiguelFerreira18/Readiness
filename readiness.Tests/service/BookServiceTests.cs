using readiness.service;
using Moq;
using Xunit;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _repo = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly BookService _sut;

    public BookServiceTests() => _sut = new BookService(_userServiceMock.Object, _repo.Object);
    
    private static CreateBookRequest CreateABookRequest() =>
        new(
            Name: "The tree of knowledge",
            Summary: "This is a good book",
            Publisher: "Great book publisher",
            YearOfRelease: 1958,
            NumberOfPage: 850,
            Isbn: 1010101101L,
            []
        );
    
    [Fact]
    public async Task CreateAsync_UserIsAdmin_NoExistingBook_AddsAndReturnsResponse()
    {

        CreateBookRequest request = CreateABookRequest();
        long userId = 1L;
        long generatedBookId = 10L;
        var expectedResponse = new BookResponse(
            Id: generatedBookId,
            Name: request.Name,
            Summary: request.Summary,
            Publisher: request.Publisher,
            YearOfRelease: request.YearOfRelease,
            NumberOfPage: request.NumberOfPage,
            Isbn: request.Isbn,
            CategoryIds: request.CategoryIds
        );        
        _userServiceMock.Setup(u => u.IsUserModerator(userId))
            .ReturnsAsync(true);
        _repo.Setup(r => r.GetBookByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(expectedResponse);
        
        BookResponse response = await _sut.CreateAsync(request, userId);

        Assert.Same(expectedResponse, response);
        _repo.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
    }
    
    
    [Fact]
    public async Task CreateAsync_UserIsAdmin_ReturnsNullAndDoesNotAdd()
    {
        var request = CreateABookRequest();
        long userId = 1L;
        _userServiceMock.Setup(u => u.IsUserModerator(userId))
            .ReturnsAsync(false);

        var result = await _sut.CreateAsync(request, userId);

        Assert.Null(result);
        _repo.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
    }
}
