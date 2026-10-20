using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BookCategory> BookCategorys => Set<BookCategory>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<BookList> BookLists => Set<BookList>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<string>()
        .HasMaxLength(20);
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fantasy" },
            new Category { Id = 2, Name = "Sci-Fi" },
            new Category { Id = 3, Name = "Romance" },
            new Category { Id = 4, Name = "Classic" },
            new Category { Id = 5, Name = "Adventure" },
            new Category { Id = 6, Name = "Historic" },
            new Category { Id = 7, Name = "Manga" },
            new Category { Id = 8, Name = "Comic" }
        );
    }
}
