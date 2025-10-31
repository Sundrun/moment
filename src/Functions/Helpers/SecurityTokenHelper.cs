using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Helpers;

public static class SecurityTokenHelper
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    
    public static bool TryExtractToken(this HttpRequestData request, out string token)
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
        
        token = authHeader.Substring("Bearer ".Length).Trim();
        return true;
        
        // TODO test this
    }
}