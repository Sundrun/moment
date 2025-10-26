using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class MomentContext : DbContext
{
    public DbSet<CoreMoment> Moments { get; set; }
    // public DbSet<MomentOwner> MomentOwners { get; set; }
    // public DbSet<MomentOwnership> MomentOwnerships { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("SqlConnectionString is not configured.");
        }
        
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoreMoment>()
            .Property(e => e.Id)
            .HasConversion(
                v => v.Id,
                v => new CoreMomentId(v));
        
        modelBuilder.Entity<CoreMoment>()
            .Property(e => e.Timestamp)
            .HasConversion(
                v => v.Timestamp,
                v => new CoreMomentTimestamp(v));
        
        // base.OnModelCreating(modelBuilder); // TODO is this needed?
    }
}