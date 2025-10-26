using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class MomentContext(DbContextOptions<MomentContext> options) : DbContext(options)
{
    public DbSet<CoreMoment> CoreMoments { get; set; }
    // public DbSet<MomentOwner> MomentOwners { get; set; }
    // public DbSet<MomentOwnership> MomentOwnerships { get; set; }

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
        
        base.OnModelCreating(modelBuilder);
    }
}