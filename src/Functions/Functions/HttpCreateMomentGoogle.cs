using System.Net;
using Functions.ValidateToken;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Functions;

public class HttpCreateMomentGoogle(IValidateToken validateToken)
{
    [Function(nameof(CreateMomentGoogle))]
    public async Task<HttpResponseData> CreateMomentGoogle([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        return request.CreateResponse(HttpStatusCode.Unauthorized);
    }
}