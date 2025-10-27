using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MomentApi;

public class HttpCreateUserFunction
{
    [Function(nameof(CreateUser))]
    public HttpResponseData CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        if (!req.Identities.Any())
        {
            return req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        }
        
        var identity = req.Identities.First();
        var principal = new ClaimsPrincipal(identity);
        
        return req.CreateResponse(System.Net.HttpStatusCode.OK);
    }
}