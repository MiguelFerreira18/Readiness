using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Review
{
    [Key]
    public long Id { get; set; }

    [MaxLength(4000)]
    public string? Text { get; set; }

    [Range(1, 5)]
    public int Star { get; set; }

    public float TimeToRead { get; set; }

    public DateTime DateOfReview { get; set; } = DateTime.UtcNow;

    public long BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; } = null!;

    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
