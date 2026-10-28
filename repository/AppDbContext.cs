using Microsoft.EntityFrameworkCore;
using readiness.model;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<UserBook> BookLists => Set<UserBook>();
    public DbSet<AuthorBook> AuthorBooks => Set<AuthorBook>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Status> Status => Set<Status>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
        .Property(u => u.Type)
        .HasConversion<string>()
        .HasMaxLength(20);

        modelBuilder.Entity<Review>()
        .HasQueryFilter(r => !r.IsDeleted);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Type =  CategoryType.Fantasy },
            new Category { Id = 2, Type = CategoryType.SciFi },
            new Category { Id = 3, Type = CategoryType.Romance },
            new Category { Id = 4, Type = CategoryType.Classic },
            new Category { Id = 5, Type = CategoryType.Adventure },
            new Category { Id = 6, Type = CategoryType.Historic },
            new Category { Id = 7, Type = CategoryType.Manga },
            new Category { Id = 8, Type = CategoryType.Comic }
        );
    }
}
