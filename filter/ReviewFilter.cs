public class ReviewFilter
{
    public float? TimeToReadStart { get; set; }
    public float? TimeToReadEnd { get; set; }
    public DateTime? DateOfReviewStart { get; set; }
    public DateTime? DateOfReviewFinish { get; set; }
    public int? StarStart { get; set; }
    public int? StarFinish { get; set; }

    public ReviewFilter(){
        
    }

    public ReviewFilter(float? timeToReadStart, float? timeToReadEnd, DateTime? dateOfReviewStart, DateTime? dateOfReviewFinish, int? starStart, int? starFinish)
    {
        TimeToReadStart = timeToReadStart;
        TimeToReadEnd = timeToReadEnd;
        DateOfReviewStart = dateOfReviewStart;
        DateOfReviewFinish = dateOfReviewFinish;
        StarStart = starStart;
        StarFinish = starFinish;
    }

    public bool Matches(ReviewResponse review)
    {
        return timeToReadMatch(review)
        && dateOfReviewMatch(review)
        && startStartMatch(review);
    }

    private bool timeToReadMatch(ReviewResponse review)
    {
        if (TimeToReadStart.HasValue && TimeToReadStart > review.TimeToRead) return false;
        if (TimeToReadEnd.HasValue && TimeToReadEnd < review.TimeToRead) return false;
        return true;
    }

    private bool dateOfReviewMatch(ReviewResponse review)
    {
        if (DateOfReviewStart.HasValue && DateOfReviewStart > review.DateOfReview) return false;
        if (DateOfReviewFinish.HasValue && DateOfReviewFinish < review.DateOfReview) return false;
        return true;
    }

    private bool startStartMatch(ReviewResponse review)
    {
        if (StarStart.HasValue && StarStart > review.Star) return false;
        if (StarFinish.HasValue && StarFinish < review.Star) return false;
        return true;
    }
}


