using Microsoft.EntityFrameworkCore;
using Fm.Api.Entities;

namespace Fm.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // 사용자
    public DbSet<User> Users => Set<User>();
    // 가계부
    public DbSet<Transaction> Transactions => Set<Transaction>();
    // 게시글
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}

