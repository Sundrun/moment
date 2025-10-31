using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;

namespace Functions.Helpers;

public static class SecurityTokenHelper
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    
    public static bool TryExtractToken(this HttpRequestData request, out SecurityToken token)
    {
        token = null!;
        
        if (!request.Headers.TryGetValues("Authorization", out var authHeaderValues))
        {
            return false;
        }
        
        var authHeader = authHeaderValues.FirstOrDefault();
        if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        
        var jwtString = authHeader.Substring("Bearer ".Length).Trim();

        token = TokenHandler.ReadJwtToken(jwtString);
        return true;
        
        // TODO test this
    }
}