using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class ModelBuilderMomentOwnerExtension
{
    public static void BuildMomentOwner(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentOwner>()
            .HasKey(o => o.Id);
        
        modelBuilder.Entity<MomentOwner>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new MomentOwnerId(v));
    }
}