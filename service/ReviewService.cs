
// Better for testing
public interface IReviewService
{
    Task<List<ReviewResponse>> GetAllAsync();
    Task<ReviewResponse?> GetByIdAsync(long id);
    Task<ReviewResponse?> CreateAsync(CreateReviewRequest request, long userId);
    Task<ReviewResponse?> UpdateAsync(long userId, long id, UpdateReviewRequest request);
    Task<bool> DeleteAsync(long userId, long id, bool isAdmin);

}


public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;
    public ReviewService(IReviewRepository repository) => _repository = repository;

    // After getting the authentication setup, define the user id trough the claims
    // 1 will be always the Moderator
    public async Task<ReviewResponse?> CreateAsync(CreateReviewRequest request, long userId)
    {
        var review = await _repository.GetReviewByBookIdAndUserIdAsync(request.BookId, userId);
        if (review is not null) return null;

        var newReview = request.ToEntity(userId);
        await _repository.AddAsync(newReview);
        return (await _repository.GetReviewResponseByIdAsync(newReview.Id))!;
    }

    public Task<List<ReviewResponse>> GetAllAsync()
    {
        return _repository.GetAllReviewEsponseAsync();
    }

    public Task<ReviewResponse?> GetByIdAsync(long id)
    {
        return _repository.GetReviewResponseByIdAsync(id);
    }

    public async Task<ReviewResponse?> UpdateAsync(long userId, long id, UpdateReviewRequest request)
    {
        var review = await _repository.GetByIdAsync(id);

        if (review is null || review.UserId != userId) return null;

        review.wasEdited = true;
        request.ApplyTo(review);
        await _repository.SaveAsync();

        return await _repository.GetReviewResponseByIdAsync(id);
    }

    //TODO: After claims are done, isAdmin doens't have a default 
    public async Task<bool> DeleteAsync(long userId, long id, bool isAdmin = true)
    {
        var review = await _repository.GetByIdAsync(id);
        if (review is null) return false;
        if (review.UserId != userId && !isAdmin) return false;

        review.IsDeleted = true;
        await _repository.SaveAsync();
        return true;
    }
}
