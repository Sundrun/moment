using System.Net;
using Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Operations.Commands.CreateMoment;
using Operations.Queries.ValidateToken;

namespace Functions.Functions;

public class HttpCreateMomentGoogleFunction(IValidateToken validateToken, ICreateMoment createMoment) : IHttpFunction
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
        
        var createMomentResult =  await createMoment.CreateAsync(validToken);
        if (createMomentResult is MomentCreated)
        {
            return request.CreateResponse(HttpStatusCode.Created);
        }
        
        var createUserResult =  await createMoment.CreateAsync(validToken);
        return createUserResult switch
        {
            MomentCreated => request.CreateResponse(HttpStatusCode.Created),
            NoUser => request.CreateResponse(HttpStatusCode.Forbidden),
            _ => request.CreateResponse(HttpStatusCode.InternalServerError)
        };
    }
}