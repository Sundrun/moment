using System.Net;
using Functions.CreateUser;
using Functions.Helpers;
using Functions.ValidateToken;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Functions;

public class HttpCreateUserFunction(IValidateToken validateToken, ICreateUser createUser)
{
    [Function(nameof(CreateUser))]
    public async Task<HttpResponseData> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
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

        var userId = validToken.Subject;
        return request.CreateResponse(HttpStatusCode.Created);
    }
}