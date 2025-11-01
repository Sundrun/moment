using Entities.Wrappers;

namespace Entities;

public class MomentOwnership
{
    public MomentOwnershipId Id { get; init;} = null!;
    public CoreMomentId MomentId { get; init; } = null!;
    public MomentOwnerId OwnerId { get; init; } = null!;
    
    public required CoreMoment Moment { get; init; }
    public required MomentOwner Owner { get; init; }
}