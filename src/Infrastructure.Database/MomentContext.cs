using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class MomentContext(DbContextOptions<MomentContext> options) : DbContext(options)
{
    public DbSet<CoreMoment> CoreMoments { get; set; }
    public DbSet<MomentOwner> MomentOwners { get; set; }
    // public DbSet<MomentOwnership> MomentOwnerships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.BuildCoreMoment();
        modelBuilder.BuildMomentOwner();

        base.OnModelCreating(modelBuilder);
    }
}