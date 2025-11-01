using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

internal static class ModelBuildGoogleIdentityOwnersExtension
{
    internal static void BuildGoogleIdentityOwners(this ModelBuilder modelBuilder)
    {
        BuildId(modelBuilder);
        BuildGoogleIdentity(modelBuilder);
        BuildOwner(modelBuilder);
    }

    private static void BuildId(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoogleIdentityOwner>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<GoogleIdentityOwner>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new OwnerGoogleIdentityId(v));
    }
    
    private static void BuildGoogleIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoogleIdentityOwner>()
            .Property(e => e.GoogleIdentityId)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Id,
                v => new GoogleIdentityId(v))
            .IsRequired();
        
        modelBuilder.Entity<GoogleIdentityOwner>()
            .HasOne<GoogleIdentity>(o => o.GoogleIdentity)
            .WithOne()
            .HasForeignKey<GoogleIdentityOwner>(o => o.GoogleIdentityId);
    }
    
    private static void BuildOwner(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoogleIdentityOwner>()
            .Property(e => e.OwnerId)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Id,
                v => new MomentOwnerId(v))
            .IsRequired();
        
        modelBuilder.Entity<GoogleIdentityOwner>()
            .HasOne<MomentOwner>(o => o.Owner)
            .WithOne()
            .HasForeignKey<GoogleIdentityOwner>(o => o.OwnerId);
    }
}