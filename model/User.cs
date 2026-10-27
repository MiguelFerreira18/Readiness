using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("User")]
public class User
{
    [Key]
    public long Id { get; set; }
    
    [Required, EmailAddress(ErrorMessage = "Invalid Email"),]
    public string? Email { get; set; }
    
    [Required, MaxLength(64)]
    public string? Name { get; set; }
    
    [Range(0, 2027, ErrorMessage = "Enter a valid year between 0 and 2027")]
    public int YearOfBirth { get; set; }
    [Required, MaxLength(64)]
    public string? PasswordHash { get; set; }
    
    [Required] 
    public int RoleId { get; set; }
    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; }  = null!;

    public virtual List<Review> Reviews { get; set; } = new();
    public List<UserBook> BookList { get; set; } = new(); 
}
