using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class MomentContext(DbContextOptions<MomentContext> options) : DbContext(options)
{
    public DbSet<CoreMoment> CoreMoments { get; set; }
    public DbSet<MetadataTimestamp> MetadataTimestamps { get; set; }
    public DbSet<MomentOwner> MomentOwners { get; set; }
    public DbSet<MomentOwnership> MomentOwnerships { get; set; }
    public DbSet<MomentTimestamp> MomentTimestamps { get; set; }
    public DbSet<GoogleIdentity> GoogleIdentities{ get; set; }
    public DbSet<GoogleIdentityOwner> GoogleIdentityOwners { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.BuildCoreMoment();
        modelBuilder.BuilderMetadataTimestamp();
        modelBuilder.BuildMomentOwner();
        modelBuilder.BuildMomentOwnership();
        modelBuilder.BuildMomentTimestamp();
        modelBuilder.BuildGoogleIdentity();
        modelBuilder.BuildGoogleIdentityOwners();

        base.OnModelCreating(modelBuilder);
    }
}