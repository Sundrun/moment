using Entities.Wrappers;

namespace Entities;

public record CoreMoment
{
    public CoreMomentId Id { get; init; } = null!;
    public required CoreMomentTimestamp Timestamp { get; init; }
}
