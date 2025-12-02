using Entities.Wrappers;

namespace Entities;

public class MomentTimestamp
{
    public MomentTimestampId Id { get; init; } = null!;
    public MetadataTimestampId TimestampId { get; init; } = null!;
    public CoreMomentId MomentId { get; init; } = null!;
    
    public required MetadataTimestamp Timestamp { get; init; }
    public required CoreMoment Moment { get; init; }
}