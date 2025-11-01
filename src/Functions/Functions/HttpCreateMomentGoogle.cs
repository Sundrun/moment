using System.Net;
using Functions.Helpers;
using Functions.ValidateToken;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Functions;

public class HttpCreateMomentGoogle(IValidateToken validateToken)
{
    [Function(nameof(CreateMomentGoogle))]
    public async Task<HttpResponseData> CreateMomentGoogle([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        if (!request.TryExtractToken(out var token))
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        
        var validationResult = await validateToken.ValidateTokenAsync(token);
        if (validationResult is not ValidToken validToken)
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        
        return request.CreateResponse(HttpStatusCode.Created);
    }
}