using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using MomentApi.CreateUser;
using MomentApi.Helpers;

namespace MomentApi.Functions;

public class HttpCreateUserFunction(ICreateUser createUser)
{
    [Function(nameof(CreateUserAsync))]
    public async Task<HttpResponseData> CreateUserAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        if (!ToClaimsPrincipalHelper.TryConvertToClaimsPrincipal(req, out var principal))
        {
            return req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        }

        var result = await createUser.CreateAsync(principal);
        var responseCode = ToStatusCodeHelper.ToStatusCode(result);
        
        return req.CreateResponse(responseCode);
    }
}