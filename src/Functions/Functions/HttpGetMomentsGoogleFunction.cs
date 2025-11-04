using System.Net;
using Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Operations.Queries.GetMoments;
using Operations.Queries.ValidateToken;

namespace Functions.Functions;

public class HttpGetMomentsGoogleFunction(IValidateToken validateToken, IGetMoments getMoments) : IHttpFunction
{
    [Function(nameof(GetMomentsGoogle))]
    public async Task<HttpResponseData> GetMomentsGoogle([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request)
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
        
        throw new NotImplementedException();
    }
}