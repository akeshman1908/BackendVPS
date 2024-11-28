using backend.Domain;
using Microsoft.EntityFrameworkCore;

namespace backend.Repo.DbContext;


public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Specificeer de tabelnamen
        modelBuilder.Entity<User>().ToTable("gebruiker");
        modelBuilder.Entity<Role>().ToTable("rol");
    }
}
