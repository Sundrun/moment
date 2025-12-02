using Entities.Wrappers;

namespace Entities;

public class MetadataTimestamp
{
    public MetadataTimestampId Id { get; init; } = null!;
    public MetadataTimestampDateTimeOffset Timestamp { get; init; } = null!;
}