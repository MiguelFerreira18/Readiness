using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Role")]
public class Role
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public RoleType Type { get; set; }
}