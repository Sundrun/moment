using Entities.Wrappers;

namespace Entities;

public class OwnerGoogleIdentity
{
    public OwnerGoogleIdentityId Id { get; init; } = null!;
    public MomentOwnerId OwnerId { get; init; } = null!;
    public OwnerGoogleIdentitySubject Subject { get; init; } = null!;
    public required MomentOwner Owner { get; init; }
}