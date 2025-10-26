namespace Entities;

public class MomentOwnership
{
    public required MomentOwnershipId Id { get; init;}
    public required CoreMomentId MomentId { get; init; }

    public required MomentOwnerId OwnerId { get; init; }
    
    public CoreMoment CoreMoment { get; init; } = null!;
}