using Entities.Wrappers;

namespace Entities;

public class MomentOwnership
{
    public required MomentOwnershipId Id { get; init;}
    public required CoreMomentId MomentId { get; init; }
    public required MomentOwnerId OwnerId { get; init; }
    
    public CoreMoment Moment { get; init; } = null!;
    public MomentOwner Owner { get; init; } = null!;
}