using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Extensions;

internal static class ModelBuilderMomentTimestampExtension
{
    internal static void BuildMomentTimestamp(this ModelBuilder modelBuilder)
    {
        BuildId(modelBuilder);
        BuildTimestamp(modelBuilder);
        BuildMoment(modelBuilder);
    }

    private static void BuildId(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentTimestamp>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<MomentTimestamp>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new MomentTimestampId(v));
    }
    
    private static void BuildTimestamp(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentTimestamp>()
            .Property(e => e.TimestampId)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Id,
                v => new MetadataTimestampId(v))
            .IsRequired();
        
        modelBuilder.Entity<MomentTimestamp>()
            .HasOne<MetadataTimestamp>(o => o.Timestamp)
            .WithOne()
            .HasForeignKey<MomentTimestamp>(o => o.TimestampId);
    }
    
    private static void BuildMoment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MomentTimestamp>()
            .Property(e => e.MomentId)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Id,
                v => new CoreMomentId(v))
            .IsRequired();

        modelBuilder.Entity<MomentTimestamp>()
            .HasOne<CoreMoment>(o => o.Moment)
            .WithOne()
            .HasForeignKey<MomentTimestamp>(o => o.MomentId);
    }
}