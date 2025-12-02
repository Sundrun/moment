using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Extensions;

internal static class ModelBuildGoogleIdentityExtension
{
    internal static void BuildGoogleIdentity(this ModelBuilder modelBuilder)
    {
        BuildId(modelBuilder);
        BuildSubject(modelBuilder);
    }

    private static void BuildId(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoogleIdentity>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<GoogleIdentity>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new GoogleIdentityId(v));
    }

    private static void BuildSubject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoogleIdentity>()
            .HasIndex(e => e.Subject)
            .IsUnique();
        
        modelBuilder.Entity<GoogleIdentity>()
            .Property(e => e.Subject)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Subject,
                v => new GoogleIdentitySubject(v));
    }
}