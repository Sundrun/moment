namespace Entities;

public class MomentOwnership(MomentOwnershipId id, CoreMomentId momentId, MomentOwnerId ownerId)
{
    public MomentOwnershipId Id { get; set;} = id;
    public CoreMomentId MomentId { get; set; } = momentId;

    public MomentOwnerId OwnerId { get; set; } = ownerId;
    // public CoreMoment CoreMoment { get; set; }
}