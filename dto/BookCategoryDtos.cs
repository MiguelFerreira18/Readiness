namespace readiness.dto;

public record BookCategoryResponse(
    long Id,
    long BookId,
    string Category
);

// Only if we decide to allow POST of new Book Category
// public record CreateBookCategoryRequest(
//     [Required] long BookId,
//     [Required] CategoryType Category
// );