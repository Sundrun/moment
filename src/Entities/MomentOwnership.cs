namespace Entities;

public class MomentOwnership
{
    public MomentOwnershipId Id { get; init;}
    public CoreMomentId MomentId { get; init; }

    public MomentOwnerId OwnerId { get; init; }
    public CoreMoment CoreMoment { get; init; }
}