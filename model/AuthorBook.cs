namespace readiness.model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AuthorBook
{
    [Key] 
    public long Id { get; set; }
    
    [Required] 
    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; } = null!;
    
    [Required] 
    public long BookId { get; set; }

    [ForeignKey(nameof(BookId))] public Book Book { get; set; } = null!;
}