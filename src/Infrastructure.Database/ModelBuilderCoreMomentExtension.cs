using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class ModelBuilderCoreMomentExtension
{
    public static void BuildCoreMoment(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoreMoment>()
            .HasKey(o => o.Id);
        
        modelBuilder.Entity<CoreMoment>()
            .Property(e => e.Id)
            .HasConversion(
                v => v.Id,
                v => new CoreMomentId{Id = v});

        modelBuilder.Entity<CoreMoment>()
            .Property(e => e.Timestamp)
            .HasConversion(
                v => v.Timestamp,
                v => new CoreMomentTimestamp{Timestamp = v});
    }
}