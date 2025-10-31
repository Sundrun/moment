using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Functions;
using Functions.ValidateToken;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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

            return new ValidToken(validatedToken.Subject);
        }
        catch
        {
            return new InvalidToken();
        }
    }
}