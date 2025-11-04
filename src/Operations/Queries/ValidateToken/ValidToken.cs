using Entities.Wrappers;

namespace Operations.Queries.ValidateToken;

public record ValidToken(GoogleIdentitySubject Subject) : IValidatedToken;