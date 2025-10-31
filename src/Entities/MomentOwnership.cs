using Entities.Wrappers;

namespace Entities;

public class MomentOwnership
{
    public MomentOwnershipId Id { get; init;} = null!;
    public required CoreMomentId MomentId { get; init; }
    public required MomentOwnerId OwnerId { get; init; }
    
    public CoreMoment Moment { get; init; } = null!;
    public MomentOwner Owner { get; init; } = null!;
}