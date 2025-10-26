using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class ModelBuilderMomentOwnershipExtension
{
    public static void BuildMomentOwnership(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentOwnership>()
            .HasKey(o => o.Id);
        
        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.Id)
            .HasConversion(
                v => v.Id,
                v => new MomentOwnershipId(v));
        
        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.MomentId)
            .HasConversion(
                v => v.Id,
                v => new CoreMomentId(v));
        
        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.OwnerId)
            .HasConversion(
                v => v.Id,
                v => new MomentOwnerId(v));
    }
}