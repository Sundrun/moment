namespace Entities;

public class MomentOwnership
{
    public MomentOwnershipId Id { get; set;}
    public CoreMomentId MomentId { get; set; }

    public MomentOwnerId OwnerId { get; set; }
    public CoreMoment CoreMoment { get; set; }
}