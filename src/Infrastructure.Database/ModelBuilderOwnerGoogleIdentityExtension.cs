using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

internal static class ModelBuilderOwnerGoogleIdentityExtension
{
    internal static void BuildOwnerGoogleIdentity(this ModelBuilder modelBuilder)
    {
        BuildId(modelBuilder);
        BuildOwner(modelBuilder);
        BuildSubject(modelBuilder);
    }

    private static void BuildId(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OwnerGoogleIdentity>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<OwnerGoogleIdentity>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new OwnerGoogleIdentityId(v));
    }
    
    private static void BuildOwner(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OwnerGoogleIdentity>()
            .Property(e => e.OwnerId)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Id,
                v => new MomentOwnerId(v))
            .IsRequired();
    }

    private static void BuildSubject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OwnerGoogleIdentity>()
            .Property(e => e.Subject)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Subject,
                v => new OwnerGoogleIdentitySubject(v));
    }
}