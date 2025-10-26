using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class ModelBuilderMomentOwnershipExtension
{
    public static void BuildMomentOwnership(this ModelBuilder modelBuilder)
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
            .HasConversion(
                v => v.Id,
                v => new MomentOwnershipId{Id = v});
    }
    
    private static void BuildMoment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.MomentId)
            .HasConversion(
                v => v.Id,
                v => new CoreMomentId{Id = v});

        // modelBuilder.Entity<MomentOwnership>()
        //     .HasOne<CoreMoment>(o => o.CoreMoment)
        //     .WithOne()
        //     .HasForeignKey<MomentOwnership>(o => o.MomentId);
        //     // .HasForeignKey<MomentOwnership>(nameof(MomentOwnership.MomentId));
    }
    
    private static void BuildOwner(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentOwnership>()
            .Property(e => e.OwnerId)
            .HasConversion(
                v => v.Id,
                v => new MomentOwnerId{Id = v});

        // modelBuilder.Entity<MomentOwnership>()
        //     .HasOne<MomentOwner>(o => o.MomentOwner)
        //     .WithOne()
        //     .HasForeignKey<MomentOwnership>(o => o.OwnerId);
    }
}