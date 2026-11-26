using System.ComponentModel.DataAnnotations;
public record CreateBookRequest(
    [Required, MaxLength(255)]
    string Name,

    [MaxLength(4000)]
    string? Summary,

    [Required, MaxLength(150)]
    string Publisher,

    [Required]
    int YearOfRelease,

    [Required, Range(1, int.MaxValue)]
    int NumberOfPage,

    [Required]
    long Isbn,

    List<String>? Categories
);

public record BookResponse(
    long Id,
    string Name,
    string? Summary,
    string Publisher,
    int YearOfRelease,
    int NumberOfPage,
    long Isbn,
    List<String>? Categories
);