namespace Entities;

public class CoreMomentTimestamp(DateTimeOffset timestamp)
{
    public DateTimeOffset Timestamp { get; set; } = timestamp;
}