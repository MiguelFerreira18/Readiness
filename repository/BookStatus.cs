namespace readiness.repository;

public class BookStatus
{
    public int Id { get; set; }
    
    // Usamos 'long' para combinar exatamente com o Id da tua classe Book
    public long BookId { get; set; } 

    // Aqui guardamos o tipo de estado: "Wish", "Read" ou "ToRead"
    public string StatusType { get; set; } 
}