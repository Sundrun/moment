using Entities.Wrappers;
using Functions.ValidateToken;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Authentication;

public class ValidateToken : IValidateToken
{
    private readonly GoogleJsonWebSignature.ValidationSettings _validationSettings;

    public ValidateToken(IConfiguration config)
    {
        var googleConfig = config.GetSection("Google");
        var clientId = googleConfig.GetValue<string>("ClientId")!;
        
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