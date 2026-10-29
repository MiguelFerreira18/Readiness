using System.ComponentModel.DataAnnotations;

public record CreateReviewRequest(
    [Required] long BookId,
    [MaxLength(4000)] string? Text,
    [Range(1, 5)] int Star,
    [Range(0.01, float.MaxValue)] float TimeToRead
    );

public record UpdateReviewRequest(
    [MaxLength(4000)] string Text,
    [Range(1, 5)] int Star,
    [Range(0.01, float.MaxValue)] float TimeToRead
);

public record ReviewResponse(
    long id,
    string? text,
     int Star,
     long BookId,
     float TimeToRead,
    string UserName,
    DateTime DateOfReview
    );
