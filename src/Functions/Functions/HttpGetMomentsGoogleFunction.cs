using System.Net;
using System.Text.Json;
using Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Operations.Queries.GetMoments;
using Operations.Queries.ValidateToken;

namespace Functions.Functions;

public class HttpGetMomentsGoogleFunction(IValidateToken validateToken, IGetMoments getMoments) : IHttpFunction
{
    [Function(nameof(GetMomentsGoogle))]
    public async Task<HttpResponseData> GetMomentsGoogle(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request,
        CancellationToken cancellationToken)
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
        
        var getMomentsResult =  await getMoments.GetMomentsAsync(validToken, cancellationToken);
        if (getMomentsResult is NoUser)
        {
            return request.CreateResponse(HttpStatusCode.BadRequest);
        }

        if (getMomentsResult is NoMoments)
        {
            return request.CreateResponse(HttpStatusCode.NoContent);
        }

        if (getMomentsResult is UserMoments userMoments)
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            
            var serializedMoments = JsonSerializer.Serialize(userMoments.Moments);
            
            await response.WriteStringAsync(serializedMoments);

            return response;
        }

        return request.CreateResponse(HttpStatusCode.InternalServerError);
    }
}