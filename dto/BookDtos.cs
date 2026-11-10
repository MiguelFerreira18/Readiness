using System.ComponentModel.DataAnnotations;
//TODO:DELETE ON MERGE SINCE THIS IS A MOCK UP

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

    List<long>? CategoryIds
);

public record BookResponse(
    long Id,
    string Name,
    string? Summary,
    string Publisher,
    int YearOfRelease,
    int NumberOfPage,
    long Isbn,
    List<string>? Categories
);