using Entities.Wrappers;

namespace Functions.ValidateToken;

public record ValidToken(GoogleIdentitySubject Subject) : IValidatedToken;