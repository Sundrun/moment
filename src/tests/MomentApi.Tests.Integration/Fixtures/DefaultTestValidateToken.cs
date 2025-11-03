using Entities.Wrappers;
using Operations.Queries.ValidateToken;

namespace MomentApi.Tests.Integration.Fixtures;

public class DefaultTestValidateToken : IValidateToken
{
    public static GoogleIdentitySubject TestSubject { get; } = new("TestSubject");
    
    public Task<IValidatedToken> ValidateTokenAsync(string token) => Task.FromResult<IValidatedToken>(new ValidToken(TestSubject));
}