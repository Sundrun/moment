namespace Entities;

public record CoreMoment
{
    public required CoreMomentId Id { get; init; }
    public required CoreMomentTimestamp Timestamp { get; init; }
}
