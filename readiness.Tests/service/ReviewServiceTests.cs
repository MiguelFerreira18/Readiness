
using Moq;
using Xunit;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _repo = new();
    private readonly ReviewService _sut;

    public ReviewServiceTests() => _sut = new ReviewService(_repo.Object);

    private static CreateReviewRequest ACreateRequest(
        long bookId = 10,
        string? text = "Great book",
        int star = 5,
        float timeToRead = 1.5f) =>
        new(bookId, text, star, timeToRead);

    private static UpdateReviewRequest AnUpdateRequest(
        string text = "Updated text",
        int star = 4,
        float timeToRead = 2f) =>
        new(text, star, timeToRead);

    private static ReviewResponse AResponse(
        long id = 1,
        string? text = "Great book",
        int star = 5,
        long bookId = 10,
        float timeToRead = 1.5f,
        string userName = "user",
        DateTime? dateOfReview = null) =>
        new(id, text, star, bookId, timeToRead, userName, dateOfReview ?? new DateTime(2026, 1, 1));

    [Fact]
    public async Task CreateAsync_UserAlreadyReviewdBook_ReturnsNullAndDoesNotAdd()
    {
        long bookId = 10L;
        var request = ACreateRequest(bookId: bookId);
        _repo.Setup(r => r.GetReviewByBookIdAndUserIdAsync(bookId, 1L))
        .ReturnsAsync(new Review { Id = 50L, UserId = 1L });

        var result = await _sut.CreateAsync(request, userId: 1L);

        Assert.Null(result);
        _repo.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_NoExistingReview_AddsAndReturnsResponse()
    {
        long bookId = 10L;
        var request = ACreateRequest(bookId: bookId);
        var response = AResponse();

        _repo.Setup(r => r.GetReviewByBookIdAndUserIdAsync(bookId, 1L))
        .ReturnsAsync((Review?)null);
        _repo.Setup(r => r.GetReviewResponseByIdAsync(It.IsAny<long>()))
        .ReturnsAsync(response);

        var result = await _sut.CreateAsync(request, userId: 1L);

        Assert.Same(response, result);
        _repo.Verify(r => r.AddAsync(It.Is<Review>(x => x.UserId == 1L)), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsWhatRepositoryReturns()
    {
        var response = new List<ReviewResponse>{
            AResponse(),
            AResponse(id: 2L, star:3),
            AResponse(id: 3L, userName: "other")
        };
        _repo.Setup(r => r.GetAllReviewEsponseAsync())
        .ReturnsAsync(response);

        var result = await _sut.GetAllAsync();

        Assert.Same(response, result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsWhatRepositoryReturns()
    {
        long reviewId = 10L;
        var response = AResponse(id: reviewId);
        _repo.Setup(r => r.GetReviewResponseByIdAsync(reviewId))
        .ReturnsAsync(response);

        var result = await _sut.GetByIdAsync(reviewId);

        Assert.Same(response, result);
    }

    [Fact]
    public async Task UpdateAsync_ReviewBelongToAnotherUser_ReturnsNullAndDoesNotSave()
    {
        var request = AnUpdateRequest();
        long reviewId = 10L;
        long userId = 1L;

        _repo.Setup(r => r.GetByIdAsync(reviewId))
        .ReturnsAsync(new Review { Id = reviewId, UserId = 2L });

        var result = await _sut.UpdateAsync(userId, reviewId, request);

        Assert.Null(result);
        _repo.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReviewNotFound_ReturnsNullAndDoesNotSave()
    {
        var request = AnUpdateRequest();
        long reviewId = 10L;
        long userId = 1L;

        _repo.Setup(r => r.GetByIdAsync(reviewId))
        .ReturnsAsync((Review?)null);

        var result = await _sut.UpdateAsync(userId, reviewId, request);

        Assert.Null(result);
        _repo.Verify(r => r.SaveAsync(), Times.Never);
    }


    [Fact]
    public async Task UpdateAsync_Owner_SavesAndReturnsResponse()
    {
        var request = AnUpdateRequest();
        var response = AResponse();
        long reviewId = 1L;
        long userId = 1L;

        _repo.Setup(r => r.GetByIdAsync(reviewId))
        .ReturnsAsync(new Review { Id = reviewId, UserId = userId });
        _repo.Setup(r => r.GetReviewResponseByIdAsync(reviewId))
        .ReturnsAsync(response);

        var result = await _sut.UpdateAsync(userId, reviewId, request);

        Assert.Same(response, result);
        _repo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReviewNotFound_ReturnsFalseAndDoesNotSave()
    {
        long reviewId = 1L;
        long userId = 1L;

        _repo.Setup(r => r.GetByIdAsync(reviewId))
        .ReturnsAsync((Review?)null);

        var result = await _sut.DeleteAsync(userId, reviewId, false);

        Assert.False(result);
        _repo.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NotOwnerAndNotAdmin_ReturnsFalseAndDoesNotSave()
    {
        long reviewId = 1L;
        long userId = 1L;
        var review = new Review { Id = reviewId, UserId = 3L };

        _repo.Setup(r => r.GetByIdAsync(reviewId))
        .ReturnsAsync(review);

        var result = await _sut.DeleteAsync(userId, reviewId, false);

        Assert.False(result);
        Assert.False(review.IsDeleted);
        _repo.Verify(r => r.SaveAsync(), Times.Never);
    }



    [Fact]
    public async Task DeleteAsync_NotOwnerButAdmin_ReturnsTrueAndDeletes()
    {
        long reviewId = 1L;
        long userId = 1L;
        var review = new Review { Id = reviewId, UserId = 50L };

        _repo.Setup(r => r.GetByIdAsync(reviewId))
        .ReturnsAsync(review);

        var result = await _sut.DeleteAsync(userId, reviewId, true);

        Assert.True(result);
        Assert.True(review.IsDeleted);
        _repo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_OwnerButNotAdmin_ReturnsTrueAndDeletes()
    {
        long reviewId = 1L;
        long userId = 1L;
        var review = new Review { Id = reviewId, UserId = 1L };

        _repo.Setup(r => r.GetByIdAsync(reviewId))
        .ReturnsAsync(review);

        var result = await _sut.DeleteAsync(userId, reviewId, false);

        Assert.True(result);
        Assert.True(review.IsDeleted);
        _repo.Verify(r => r.SaveAsync(), Times.Once);
    }
}
