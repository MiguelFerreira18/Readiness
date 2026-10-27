using System.ComponentModel.DataAnnotations;

namespace readiness.model;

public class Status
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public UserBookType Type {get; set;}
}