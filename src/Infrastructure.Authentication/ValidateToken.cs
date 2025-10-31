using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Functions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class ValidateToken : IValidateToken
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly TokenValidationParameters _validationParameters;

    public ValidateToken(IConfiguration config)
    {
        var googleConfig = config.GetSection("Google");
        var clientId = googleConfig.GetValue<string>("ClientId")!;
        var clientSecret = googleConfig.GetValue<string>("ClientSecret")!;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret));
        
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = clientId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey
        };
    }

    public Task<TokenValidationResult> ValidateTokenAsync(SecurityToken token) => _tokenHandler.ValidateTokenAsync(token, _validationParameters);
}