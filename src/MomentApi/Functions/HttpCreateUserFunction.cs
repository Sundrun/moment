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
        if (!req.Identities.Any())
        {
            return req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        }
        
        var identity = req.Identities.First();
        var principal = new ClaimsPrincipal(identity);

        var result = await createUser.CreateAsync(principal);
        var responseCode = ToStatusCodeHelper.ToStatusCode(result);
        
        return req.CreateResponse(responseCode);
    }
}