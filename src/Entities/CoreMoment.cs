namespace Entities;

public record CoreMoment
{
    public CoreMomentId Id { get; init; }
    public CoreMomentTimestamp Timestamp { get; init; }
}
