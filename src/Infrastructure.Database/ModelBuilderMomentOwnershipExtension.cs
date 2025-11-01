using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

internal static class ModelBuilderMomentOwnershipExtension
{
    internal static void BuildMomentOwnership(this ModelBuilder modelBuilder)
    {
        BuildId(modelBuilder);
        BuildMoment(modelBuilder);
        BuildOwner(modelBuilder);
    }

    private static void BuildId(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentOwnership>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new MomentOwnershipId(v));
    }
    
    private static void BuildMoment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.MomentId)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Id,
                v => new CoreMomentId(v))
            .IsRequired();

        modelBuilder.Entity<MomentOwnership>()
            .HasOne<CoreMoment>(o => o.Moment)
            .WithOne()
            .HasForeignKey<MomentOwnership>(o => o.MomentId);
    }
    
    private static void BuildOwner(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.OwnerId)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Id,
                v => new MomentOwnerId(v))
            .IsRequired();
    }
}