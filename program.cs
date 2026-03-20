using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

// Entities
public class User
{
    public int Id { get; set; }

    [Required]
    [MinLength(1)]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public List<Movie> Movies { get; set; } = new();
}

public class Movie
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int ReleaseYear { get; set; }

    public string? Description { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Data Source=DESKTOP-8UTPR8Q\\IAM5344;Initial Catalog=MoviesDb;Integrated Security=True;Encrypt=False;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Movie>()
            .HasOne(m => m.User)
            .WithMany(u => u.Movies)
            .HasForeignKey(m => m.UserId);
    }
}


class Program
{
    static void Main(string[] args)
    {
        using var db = new AppDbContext();
        db.Database.Migrate();
    }
}
