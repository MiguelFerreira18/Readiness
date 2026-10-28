


public static class ReviewMapper
{
    public static ReviewResponse ToResponse(this Review r) => new(
        r.Id, r.Text, r.Star, r.BookId, r.TimeToRead, r.User.Name, r.DateOfReview
    );

    public static Review ToEntity(this CreateReviewRequest request, long userId) => new()
    {
        BookId = request.BookId,
        Text = request.Text,
        Star = request.Star,
        TimeToRead = request.TimeToRead,
        UserId = userId,
        IsDeleted = false
    };

    public static void ApplyTo(this UpdateReviewRequest request, Review review)
    {
        review.Text = request.Text?.Trim();
        review.Star = request.Star;
        review.wasEdited = true;
        review.TimeToRead = request.TimeToRead;
    }

    public static IQueryable<ReviewResponse> ProjectToResponse(this IQueryable<Review> query)
     => query.Select(r => new ReviewResponse(
         r.Id, r.Text, r.Star, r.BookId, r.TimeToRead, r.User.Name, r.DateOfReview));
}
