using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;

namespace MomentApi.Helpers;

public static class ToClaimsPrincipalHelper
{
    public static bool TryConvertToClaimsPrincipal(HttpRequestData request, out ClaimsPrincipal principal)
    {
        if (!request.Identities.Any())
        {
            principal = new ClaimsPrincipal();
            return false;
        }
        
        var identity = request.Identities.First();
        principal = new ClaimsPrincipal(identity);
        return true;
    }
}