using Entities.Wrappers;
using Google.Apis.Auth;
using Operations.Queries.ValidateToken;

namespace Infrastructure.Authentication;

public class ValidateToken : IValidateToken
{
    private readonly GoogleJsonWebSignature.ValidationSettings _validationSettings;

    public ValidateToken(string clientId)
    {
        _validationSettings = new GoogleJsonWebSignature.ValidationSettings() 
        { 
            Audience = new List<string>() { clientId } 
        };
    }

    public async Task<IValidatedToken> ValidateTokenAsync(string token)
    {
        try
        {
            var validatedToken = await GoogleJsonWebSignature.ValidateAsync(token, _validationSettings);

            var subject = new GoogleIdentitySubject(validatedToken.Subject);
            return new ValidToken(subject);
        }
        catch
        {
            return new InvalidToken();
        }
    }
}