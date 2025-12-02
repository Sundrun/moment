using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Extensions;

internal static class ModelBuilderMetadataTimestampExtension
{
    internal static void BuilderMetadataTimestamp(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MetadataTimestamp>()
            .HasKey(o => o.Id);
        
        modelBuilder.Entity<MetadataTimestamp>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.Id,
                v => new MetadataTimestampId(v));

        modelBuilder.Entity<MetadataTimestamp>()
            .Property(e => e.Timestamp)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasConversion(
                v => v.Timestamp,
                v => new MetadataTimestampDateTimeOffset(v));
    }
}