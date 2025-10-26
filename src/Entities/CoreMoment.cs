namespace Entities;

public class CoreMoment(CoreMomentId id, CoreMomentTimestamp timestamp)
{
    public CoreMomentId Id { get; set; } = id;
    public CoreMomentTimestamp Timestamp { get; set; } = timestamp;
}
