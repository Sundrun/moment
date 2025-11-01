using Entities.Wrappers;

namespace Functions.ValidateToken;

public record ValidToken(OwnerGoogleIdentitySubject Subject) : IValidatedToken;