using HF.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HF.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Registration>()
            .HasIndex(r => new { r.EventId, r.Email })
            .IsUnique();
    }
}