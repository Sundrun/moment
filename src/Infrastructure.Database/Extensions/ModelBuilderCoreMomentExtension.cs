using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Extensions;

internal static class ModelBuilderCoreMomentExtension
{
    internal static void BuildCoreMoment(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoreMoment>()
            .HasKey(o => o.Id);
        
        modelBuilder.Entity<CoreMoment>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new CoreMomentId(v));

        modelBuilder.Entity<CoreMoment>()
            .Property(e => e.Timestamp)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Timestamp,
                v => new CoreMomentTimestamp(v));
    }
}