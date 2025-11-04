using Entities.Wrappers;

namespace Entities;

public class GoogleIdentityOwner
{
    public OwnerGoogleIdentityId Id { get; init; } = null!;
    public GoogleIdentityId GoogleIdentityId { get; init; } = null!;
    public MomentOwnerId OwnerId { get; init; } = null!;
    public required GoogleIdentity GoogleIdentity { get; init; }
    public required MomentOwner Owner { get; init; }
}