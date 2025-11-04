using Entities.Wrappers;

namespace Entities;

public class GoogleIdentity
{
    public GoogleIdentityId Id { get; init; } = null!;
    public GoogleIdentitySubject Subject { get; init; } = null!;
}